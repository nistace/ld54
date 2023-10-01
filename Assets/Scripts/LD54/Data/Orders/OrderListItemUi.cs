using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NiUtils.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LD54.Data {
	public class OrderListItemUi : MonoBehaviour {
		private static HashSet<OrderPackageImageUi> packageImagePool { get; } = new HashSet<OrderPackageImageUi>();

		[SerializeField] protected GameObject _itemRootObject;
		[SerializeField] protected GameObject _progressGameObject;
		[SerializeField] protected Image _background;
		[SerializeField] protected Image _expirationProgress;
		[SerializeField] protected bool _expirationForward;
		[SerializeField] protected OrderPackageImageUi _packageImagePrefab;
		[SerializeField] protected bool _useCounters;
		[SerializeField] protected Transform _packageImageParent;
		[SerializeField] protected Button _sendButton;
		[SerializeField] protected AnimationCurve _sendButtonAnimationCurve;
		[SerializeField] protected float _sendButtonAnimationSpeed = 2;
		private Order order { get; set; }
		private Dictionary<Package, List<OrderPackageImageUi>> packageImages { get; } = new Dictionary<Package, List<OrderPackageImageUi>>();

		public Order.Event onSendOrderClicked { get; } = new Order.Event();

		private void Awake() {
			packageImagePool.RemoveWhere(t => !t);
		}

		public void Setup(Order order) {
			this.order?.onExpired.RemoveListener(HandleOrderExpired);
			this.order = order;
			this.order.onExpired.AddListenerOnce(HandleOrderExpired);
			Clean();
			GeneratePackageImages();
			RefreshPackageImagesState();
			RefreshExpirationProgress();
			_itemRootObject.SetActive(true);
			_sendButton.onClick.AddListenerOnce(HandleSendButtonClicked);
			enabled = true;
		}

		private void HandleOrderExpired(Order order) {
			Clean();
			StartCoroutine(ExpireBackground());
		}

		private IEnumerator ExpireBackground() {
			var initialPosition = _background.transform.localPosition;
			var targetPosition = initialPosition + Vector3.down * GameSessionData.config.order.uiExpiredMoveDownOffset.Random();
			var targetAngle = (Mathf.RoundToInt(Random.value) * 2 - 1) * GameSessionData.config.order.uiExpiredRotateMaxAngle.Random();
			var targetColor = GameSessionData.config.order.uiExpiredOrderColor;

			if (GameSessionData.config.order.uiExpiredAnimationDuration > 0) {
				for (var lerp = 0f; lerp < 1; lerp += Time.deltaTime / GameSessionData.config.order.uiExpiredAnimationDuration) {
					_background.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, lerp);
					_background.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, targetAngle, lerp));
					_background.color = Color.Lerp(Color.white, targetColor, lerp);
					yield return null;
				}
			}

			_background.transform.localPosition = targetPosition;
			_background.transform.localRotation = Quaternion.Euler(0, 0, targetAngle);
			_background.color = targetColor;
		}

		private void HandleSendButtonClicked() => onSendOrderClicked.Invoke(order);

		private void OnEnable() {
			OrderStorageObserver.onStorageStateChanged.AddListenerOnce(HandleStorageStateChanged);
			HandleStorageStateChanged();
		}

		private void OnDisable() {
			OrderStorageObserver.onStorageStateChanged.RemoveListener(HandleStorageStateChanged);
		}

		private void HandleStorageStateChanged() {
			if (order is not { isActive: true }) return;
			RefreshPackageImagesState();
			_sendButton.interactable = order != null && order.IsReadyToBeDelivered();
		}

		private void GeneratePackageImages() {
			packageImages.Clear();
			packageImages.AddRange(order.type.amounts.Select(t => t.package).Distinct().Select(t => new KeyValuePair<Package, List<OrderPackageImageUi>>(t, new List<OrderPackageImageUi>())));
			foreach (var packageAmount in order.type.amounts) {
				for (var i = 0; i < packageAmount.amount; ++i) {
					if (_useCounters && i > 1) continue;
					if (packageImagePool.Count == 0) packageImagePool.Add(Instantiate(_packageImagePrefab, _packageImageParent));
					var packageImage = packageImagePool.First();
					packageImagePool.Remove(packageImage);
					packageImage.transform.SetParent(_packageImageParent);
					packageImage.icon = packageAmount.package.type.icon;
					packageImage.gameObject.SetActive(true);
					packageImages[packageAmount.package].Add(packageImage);
				}
			}
		}

		private void RefreshPackageImagesState() {
			foreach (var package in packageImages.Keys) {
				var readyCount = order.isActive ? order.GetReadyPackageCount(package) : 0;
				var targetCount = order.isActive ? order.GetRequiredAmount(package) : 0;
				for (var i = 0; i < packageImages[package].Count; ++i) {
					var packageImage = packageImages[package][i];
					packageImage.counterText = _useCounters ? $"{readyCount} / {targetCount}" : string.Empty;
					packageImage.counterTextColor = readyCount >= targetCount ? GameSessionData.config.order.uiCounterValidColor : GameSessionData.config.order.uiCounterInvalidColor;
					packageImage.presentInStorage = i < readyCount;
				}
			}
		}

		private void Clean() {
			foreach (var packageImage in _packageImageParent.GetComponentsInChildren<OrderPackageImageUi>()) {
				packageImagePool.Add(packageImage);
				packageImage.gameObject.SetActive(false);
			}
			packageImages.Clear();
		}

		public void Disable() {
			Clean();
			enabled = false;
			_itemRootObject.SetActive(false);
		}

		private void RefreshExpirationProgress() {
			_progressGameObject.SetActive(order.isActive);
			_expirationProgress.fillAmount = _expirationForward ? order.expirationProgress : 1 - order.expirationProgress;
		}

		private void Update() {
			if (order == null) return;
			RefreshExpirationProgress();
			_sendButton.gameObject.SetActive(order.isActive);
			if (_sendButton.gameObject.activeSelf && _sendButton.interactable) {
				_sendButton.transform.localScale = Vector3.one * _sendButtonAnimationCurve.Evaluate(Time.time * _sendButtonAnimationSpeed);
			}
		}
	}
}