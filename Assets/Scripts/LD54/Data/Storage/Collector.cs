using System.Collections;
using System.Collections.Generic;
using LD54.Data;
using NiUtils.Audio;
using NiUtils.Extensions;
using UnityEngine;

public class Collector : MonoBehaviour {
	public static Collector current { get; private set; }

	[SerializeField] protected Transform _magnet;

	private bool collecting { get; set; }
	private List<Package> nextThingsToCollect { get; } = new List<Package>();

	private void Awake() {
		current = this;
	}

	public void Init() {
		transform.position = GameSessionData.config.collector.origin;
		_magnet.localPosition = new Vector3(0, GameSessionData.config.collector.magnetOriginHeight, 0);
		_magnet.localScale = Vector3.zero;
	}

	public void Collect(IEnumerable<Package> packagesToCollect) {
		foreach (var package in packagesToCollect) {
			package.Lock();
			if (!nextThingsToCollect.Contains(package)) nextThingsToCollect.Add(package);
		}
		if (!collecting) {
			collecting = false;
			StartCoroutine(Collect());
		}
	}

	private IEnumerator Collect() {
		var data = GameSessionData.config.collector;
		transform.position = Storage.current.center;
		while (nextThingsToCollect.Count > 0) {
			var packageToTake = nextThingsToCollect[0];
			nextThingsToCollect.RemoveAt(0);

			var magnetFlatOrigin = _magnet.position.With(y: 0);
			var magnetFlatTarget = packageToTake.grabAnchorPosition.With(y: 0);
			var packageGrabbed = false;
			AudioManager.Sfx.PlayRandom("collect.enter");
			for (var lerp = 0f; lerp < 1; lerp += Time.deltaTime / data.magnetAnimationDuration) {
				var magnetLerp = data.magnetYPositionCurve.Evaluate(lerp);
				var magnetY = Mathf.Lerp(data.magnetOriginHeight, packageToTake.grabAnchorPosition.y, magnetLerp);
				_magnet.position = Vector3.Lerp(magnetFlatOrigin, magnetFlatTarget, lerp * 3) + new Vector3(0, magnetY, 0);
				_magnet.localScale = Vector3.one * magnetLerp;
				if (!packageGrabbed && lerp > data.magnetGrabTimeRatio) {
					packageToTake.transform.SetParent(_magnet);
					Storage.current.RemovePackage(packageToTake);
					packageGrabbed = true;
					AudioManager.Sfx.PlayRandom("interact");
				}
				yield return null;
			}

			packageToTake.MarkAsDelivered();
			yield return null;
		}

		collecting = false;
		_magnet.localPosition = new Vector3(0, data.magnetOriginHeight, 0);
	}
}