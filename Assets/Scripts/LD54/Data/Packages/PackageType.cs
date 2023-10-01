using UnityEngine;

namespace LD54.Data {
	[CreateAssetMenu]
	public class PackageType : ScriptableObject {
		[SerializeField] protected Sprite _icon;
		[SerializeField] private PackageShape _shape;
		[SerializeField] protected PackageInWaterProps _inWater;

		public PackageShape shape => _shape;
		public PackageInWaterProps inWater => _inWater;
		public int shapeSize => _shape.shapeCellCount;
		public Sprite icon => _icon;
	}
}