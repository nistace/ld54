using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LD54.Data {
	public class TutoUi : MonoBehaviour {
		public static TutoUi current { get; set; }

		[SerializeField] protected GameObject _messageBox;
		[SerializeField] protected TMP_Text _messageText;
		[SerializeField] protected Button _messageContinueButton;
		[SerializeField] protected TutoArrow _arrow;

		public UnityEvent onContinueClicked => _messageContinueButton.onClick;

		private void Awake() {
			current = this;
			Hide();
		}

		public void Show(string message, bool showArrow, Vector3 arrowPosition) {
			_messageText.text = message;
			_arrow.gameObject.SetActive(showArrow);
			_arrow.transform.position = arrowPosition;
			_messageBox.gameObject.SetActive(true);
		}

		public void Hide() {
			_messageBox.gameObject.SetActive(false);
			_arrow.gameObject.SetActive(false);
		}
	}
}