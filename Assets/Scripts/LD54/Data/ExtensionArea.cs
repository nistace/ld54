using LD54.Data;
using UnityEngine;

public class ExtensionArea : MonoBehaviour, IInteractable {
	[SerializeField] protected Transform _quadTransform;
	[SerializeField] protected Transform _arrow;
	[SerializeField] protected Vector3 _scaleMargin = new Vector3(0, 0, .1f);

	public void RefreshOnWidth(int width, int length) {
		_quadTransform.localScale = new Vector3(1 - _scaleMargin.z, length - _scaleMargin.x, 1 - _scaleMargin.y);
		transform.position = new Vector3(width, 0, (length - 1) * .5f);
		_arrow.forward = Vector3.right;
	}

	public void RefreshOnLength(int width, int length) {
		_quadTransform.localScale = new Vector3(width - _scaleMargin.x, 1 - _scaleMargin.z, 1 - _scaleMargin.y);
		transform.position = new Vector3((width - 1) * .5f, 0, length);
		_arrow.forward = Vector3.forward;
	}
}