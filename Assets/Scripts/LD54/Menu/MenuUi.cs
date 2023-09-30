using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuUi : MonoBehaviour {
	private static MenuUi instance { get; set; }

	[SerializeField] protected Button startButton;

	public static UnityEvent onStartButtonClicked => instance.startButton.onClick;

	private void Awake() {
		instance = this;
	}
}