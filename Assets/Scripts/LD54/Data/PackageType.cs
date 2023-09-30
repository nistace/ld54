using UnityEngine;

namespace LD54.Data {
	[CreateAssetMenu]
	public class PackageType : ScriptableObject {
		[SerializeField] private PackageShape _shape;

		public PackageShape shape => _shape;
	}
}