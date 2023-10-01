using UnityEngine;
using UnityEngine.Events;

namespace LD54.Data {
	public class Package : MonoBehaviour, IInteractable {
		public class Event : UnityEvent<Package> { }

		public enum State {
			Default = 0,
			Locked = 2,
			Delivered = 3
		}

		[SerializeField] protected PackageType _type;
		[SerializeField] protected Rigidbody _rigidbody;
		[SerializeField] protected Transform _grabAnchor;

		public PackageType type => _type;
		public new Rigidbody rigidbody => _rigidbody;
		public PackageShape shape => _type.shape.GetRotated(Mathf.RoundToInt(Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up) / 90));
		private bool inWater { get; set; }
		private Vector3 waterSurfaceClosestPoint { get; set; }
		private Vector3 waterSmoothVelocity;
		public Vector3 grabAnchorPosition => _grabAnchor.position;
		public State state { get; private set; }

		public Event onDelivered { get; } = new Event();
		public Event onLocked { get; } = new Event();

		public void Init() {
			state = State.Default;
			transform.localScale = Vector3.one;
		}

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

		public void Lock() {
			state = State.Locked;
			onLocked.Invoke(this);
		}

		public void MarkAsDelivered() {
			state = State.Delivered;
			onDelivered.Invoke(this);
		}

		public bool HasSameTypeAs(Package other) => other && _type == other._type;

#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Gizmos.DrawSphere(transform.position + Vector3.up * 1, .05f);
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = Color.yellow;
			var offset = new Vector3(-(_type.shape.width - 1) * .5f, 0, -(_type.shape.length - 1) * .5f);
			for (var x = 0; x < _type.shape.width; ++x)
			for (var y = 0; y < _type.shape.length; ++y) {
				if (_type.shape[x, y]) Gizmos.DrawWireCube(offset + new Vector3(x, 0, y), Vector3.one);
			}
		}
#endif
	}
}