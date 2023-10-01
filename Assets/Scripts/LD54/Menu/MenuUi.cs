using NiUtils.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuUi : MonoBehaviour {
	private static MenuUi instance { get; set; }

	[SerializeField] protected Button startButton;
	[SerializeField] protected Slider musicVolumeSlider;
	[SerializeField] protected Slider sfxVolumeSlider;
	[SerializeField] protected Button quitButton;

	public static UnityEvent onStartButtonClicked => instance.startButton.onClick;
	public static UnityEvent<float> onMusicVolumeChanged => instance.musicVolumeSlider.onValueChanged;
	public static UnityEvent<float> onSfxVolumeChanged => instance.sfxVolumeSlider.onValueChanged;
	public static UnityEvent onQuitButtonClicked => instance.quitButton.onClick;

	private void Awake() {
		instance = this;
	}

	public static void Setup() {
		instance.musicVolumeSlider.SetValueWithoutNotify(AudioManager.Music.volume);
		instance.sfxVolumeSlider.SetValueWithoutNotify(AudioManager.Sfx.volume);
#if UNITY_WEBGL || UNITY_EDITOR
		instance.quitButton.gameObject.SetActive(false);
#endif
	}
}