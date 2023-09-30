using UnityEngine;

namespace LD54.Data {
	public class StorageCell : MonoBehaviour {
		[SerializeField] protected MeshRenderer _renderer;
		[SerializeField] protected StorageCellData _data;
		public Package package { get; set; }

		public void SetMaterial(StorageCellData.MaterialType type) => _renderer.material = _data.GetMaterial(type);
	}
}