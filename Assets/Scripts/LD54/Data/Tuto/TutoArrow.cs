using NiUtils.Types;
using UnityEngine;

public class TutoArrow : MonoBehaviour {
	[SerializeField] protected Transform _arrow;
	[SerializeField] protected FloatRange _xPositionRange = new FloatRange(-100, -20);
	[SerializeField] protected float _speed = 2;

	private void Update() {
		_arrow.localPosition = new Vector3(_xPositionRange.ValueAt(Mathf.Sin(Time.time * _speed) * .5f + .5f), 0, 0);
		var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
		transform.forward = Vector3.forward;
		transform.right = transform.position - screenCenter;
		Debug.Log(transform.position);
	}
}