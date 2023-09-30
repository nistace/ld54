using UnityEngine;

namespace LD54.Data {
	public class WaterPlane : MonoBehaviour {
		[SerializeField] protected MeshRenderer _meshRenderer;
		[SerializeField] protected int _quality;

		[ContextMenu("Build")]
		private void Build() { }
	}
}