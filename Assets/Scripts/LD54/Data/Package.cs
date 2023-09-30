using UnityEngine;

namespace LD54.Data {
	public class Package : MonoBehaviour {
		[SerializeField] protected PackageType _type;
		[SerializeField] protected Rigidbody _rigidbody;

		public new Rigidbody rigidbody => _rigidbody;
	}
}