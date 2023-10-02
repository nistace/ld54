using LD54.Data;
using NiUtils.Audio;
using NiUtils.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	private void Start() {
		MenuUi.Setup();

		MenuUi.onTutorialButtonClicked.AddListenerOnce(StartTutorial);
		MenuUi.onStartButtonClicked.AddListenerOnce(StartGame);
		MenuUi.onQuitButtonClicked.AddListenerOnce(Quit);
		MenuUi.onMusicVolumeChanged.AddListenerOnce(ChangeMusicVolume);
		MenuUi.onSfxVolumeChanged.AddListenerOnce(ChangeSfxVolume);
	}

	private static void StartTutorial() {
		GameSessionData.tutoToBePlayed = true;
		SceneManager.LoadSceneAsync("Game");
	}

	private static void ChangeMusicVolume(float value) => AudioManager.Music.volume = value;

	private static void ChangeSfxVolume(float value) {
		AudioManager.Sfx.volume = value;
		AudioManager.Sfx.PlayRandom("interact");
	}

	private static void Quit() => Application.Quit();

	private static void StartGame() {
		GameSessionData.tutoToBePlayed = false;
		SceneManager.LoadSceneAsync("Game");
	}
}