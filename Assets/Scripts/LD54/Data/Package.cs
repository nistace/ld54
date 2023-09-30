using UnityEngine;
using UnityEngine.Events;

namespace LD54.Data {
	public class Package : MonoBehaviour, IInteractable {
		public class Event : UnityEvent<Package> { }

		[SerializeField] protected PackageType _type;
		[SerializeField] protected Rigidbody _rigidbody;

		public new Rigidbody rigidbody => _rigidbody;
		public PackageShape shape => _type.shape.GetRotated(Mathf.RoundToInt(Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up) / 90));
		private bool inWater { get; set; }
		private Vector3 waterSurfaceClosestPoint { get; set; }
		private Vector3 waterSmoothVelocity;

		public static Event onDelivered { get; } = new Event();
		public static Event onDecayed { get; } = new Event();

		private void Update() {
			inWater = !_rigidbody.isKinematic && transform.position.y < _type.inWater.waterLevel;
			_rigidbody.useGravity = !inWater;
		}

		private void FixedUpdate() {
			if (inWater) {
				WaterPlane.current.GetClosestVertexPosition(transform, SetWaterClosestVertexPosition);
				var toSurfaceVector = waterSurfaceClosestPoint - transform.position;
				if (toSurfaceVector.sqrMagnitude > _type.inWater.surfaceHeight) {
					_rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, toSurfaceVector, ref waterSmoothVelocity, _type.inWater.smoothDampTime);
				}
			}
		}

		private void SetWaterClosestVertexPosition(Vector3 waterPosition) => waterSurfaceClosestPoint = waterPosition;

#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Gizmos.DrawSphere(transform.position + Vector3.up * 1, .05f);
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = Color.yellow;
			var offset = new Vector3(-(_type.shape.width - 1) * .5f, .5f, -(_type.shape.length - 1) * .5f);
			for (var x = 0; x < _type.shape.width; ++x)
			for (var y = 0; y < _type.shape.length; ++y) {
				if (_type.shape[x, y]) Gizmos.DrawCube(offset + new Vector3(x, 0, y), new Vector3(.5f, .1f, .5f));
			}
		}
#endif
	}
}