using NiUtils.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	private void Start() {
		MenuUi.onStartButtonClicked.AddListenerOnce(StartGame);
	}

	private static void StartGame() => SceneManager.LoadSceneAsync("Game");
}