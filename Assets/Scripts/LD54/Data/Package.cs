using System;
using UnityEngine;

namespace LD54.Data {
	public class Package : MonoBehaviour, IInteractable {
		[SerializeField] protected PackageType _type;
		[SerializeField] protected Rigidbody _rigidbody;

		public new Rigidbody rigidbody => _rigidbody;
		public PackageShape shape => _type.shape.GetRotated(Mathf.RoundToInt(Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up) / 90));
		public bool inWater { get; set; }

		private void Update() {
			var inWater = !_rigidbody.isKinematic && transform.position.y < -1f;
			_rigidbody.useGravity = !inWater;
			_rigidbody.AddForce(Vector3.up, ForceMode.Acceleration);
		}

#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Gizmos.DrawSphere(transform.position + Vector3.up * 1, .05f);
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = Color.yellow;
			var offset = new Vector3(-(_type.shape.width - 1) * .5f, 1, -(_type.shape.length - 1) * .5f);
			for (var x = 0; x < _type.shape.width; ++x)
			for (var y = 0; y < _type.shape.length; ++y) {
				if (_type.shape[x, y]) Gizmos.DrawCube(offset + new Vector3(x, 0, y), new Vector3(.5f, .1f, .5f));
			}
		}
#endif
	}
}