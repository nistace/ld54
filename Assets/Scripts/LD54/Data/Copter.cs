using System.Collections;
using LD54.Data;
using NiUtils.Extensions;
using UnityEngine;

public class Copter : MonoBehaviour {
	public static Copter current { get; private set; }

	[SerializeField] protected Transform _magnet;

	private bool travelling { get; set; }
	private float nextTravelTime { get; set; }

	private void Awake() {
		current = this;
	}

	public void Init() {
		transform.position = GameSessionData.current.config.copter.origin;
		_magnet.localPosition = new Vector3(0, GameSessionData.current.config.copter.magnetOriginHeight, 0);
	}

	private void Update() {
		if (!GameSessionData.isPlaying) return;
		if (travelling) return;
		nextTravelTime += Time.deltaTime / GameSessionData.current.config.copter.delaysBetweenTravels.Evaluate(GameSessionData.gameTime);
		if (nextTravelTime > 1 || Storage.current.isFull) {
			travelling = true;
			nextTravelTime = 0;
			StartCoroutine(Travel());
		}
	}

	private IEnumerator Travel() {
		var data = GameSessionData.current.config.copter;
		for (var lerp = 0f; lerp < 1; lerp += Time.deltaTime / data.travelDuration) {
			transform.position = data.GetTravelPosition(Storage.current.center, lerp);
			yield return null;
		}
		transform.position = Storage.current.center;
		for (var pickUpCount = 0;
				pickUpCount < GameSessionData.current.config.copter.GetMaxPickUpCount(GameSessionData.gameTime) && Storage.current.TryGetPackageToBeDelivered(out var packageToTake);
				++pickUpCount) {
			packageToTake.locked = true;

			var magnetFlatOrigin = _magnet.position.With(y: 0);
			var magnetFlatTarget = packageToTake.grabAnchorPosition.With(y: 0);
			var packageGrabbed = false;
			for (var lerp = 0f; lerp < 1; lerp += Time.deltaTime / data.magnetAnimationDuration) {
				var magnetLerp = data.magnetYPositionCurve.Evaluate(lerp);
				var magnetY = Mathf.Lerp(data.magnetOriginHeight, packageToTake.grabAnchorPosition.y, magnetLerp);
				_magnet.position = Vector3.Lerp(magnetFlatOrigin, magnetFlatTarget, lerp * 3) + new Vector3(0, magnetY, 0);
				_magnet.localScale = Vector3.one * magnetLerp;
				if (!packageGrabbed && lerp > data.magnetGrabTimeRatio) {
					packageToTake.transform.SetParent(_magnet);
					packageGrabbed = true;
				}
				yield return null;
			}
			Storage.current.RemovePackage(packageToTake);
			Package.onDelivered.Invoke(packageToTake);
			yield return null;
		}
		var travelBackOrigin = transform.position;
		for (var lerp = 0f; lerp < 1; lerp += Time.deltaTime / data.travelDuration) {
			transform.position = data.GetTravelPosition(travelBackOrigin, 1 - lerp);
			yield return null;
		}
		transform.position = data.origin;

		travelling = false;
		_magnet.localPosition = new Vector3(0, GameSessionData.current.config.copter.magnetOriginHeight, 0);
	}
}