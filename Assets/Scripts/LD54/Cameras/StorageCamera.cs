using LD54.Data;
using UnityEngine;

namespace LD54 {
	public class StorageCamera : MonoBehaviour {
		private static StorageCamera current { get; set; }
		public static Camera currentCamera => current._camera;

		[SerializeField] protected Storage _storage;
		[SerializeField] protected Camera _camera;
		[SerializeField] protected Transform _distanceTransform;
		[SerializeField] protected float _positionSmooth = .5f;
		[SerializeField] protected float _distanceSmooth = .5f;
		[SerializeField] protected AnimationCurve _distancePerStorageSizeCurve;
		[SerializeField] protected Vector3 _targetOffsetWithStorageCenter;

		private Vector3 positionVelocity;
		private Vector3 distanceVelocity;
		private Vector3 desiredDistanceVector;
		private int sizeUsedToEvaluateDistanceVector = -1;

		private void Awake() {
			current = this;
		}

		private void Update() {
			if (_storage.size != sizeUsedToEvaluateDistanceVector) {
				sizeUsedToEvaluateDistanceVector = _storage.size;
				desiredDistanceVector = new Vector3(0, 0, -_distancePerStorageSizeCurve.Evaluate(sizeUsedToEvaluateDistanceVector));
			}
			transform.position = Vector3.SmoothDamp(transform.position, _storage.center + _targetOffsetWithStorageCenter, ref positionVelocity, _positionSmooth);
			_distanceTransform.localPosition = Vector3.SmoothDamp(_distanceTransform.localPosition, desiredDistanceVector, ref distanceVelocity, _distanceSmooth);
		}
	}
}