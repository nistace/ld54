using UnityEngine;

namespace LD54.Data {
	[CreateAssetMenu]
	public class PackageType : ScriptableObject {
		[SerializeField] private PackageShape _shape;
		[SerializeField] protected PackageInWaterProps _inWater;

		public PackageShape shape => _shape;
		public PackageInWaterProps inWater => _inWater;
	}
}