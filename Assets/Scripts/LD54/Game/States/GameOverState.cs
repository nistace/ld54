using System.Collections;
using LD54.Data;
using NiUtils.Extensions;
using NiUtils.GameStates;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD54.Game {
	public class GameOverState : GameState {
		public static GameOverState state { get; } = new GameOverState();

		private GameOverState() { }

		protected override void Enable() {
			GameSessionData.current.EndGame();
			GameOverUi.current.Show();
		}

		protected override void Disable() { }

		protected override IEnumerator Continue() {
			yield break;
		}

		protected override void SetListenersEnabled(bool enabled) {
			GameOverUi.current.onQuitClicked.SetListenerActive(HandleQuitClicked, enabled);
			GameOverUi.current.onNewGameClicked.SetListenerActive(HandleNewGameClicked, enabled);
			GameOverUi.current.onBackToMenuClicked.SetListenerActive(HandleBackToMenuClicked, enabled);
		}

		private void HandleNewGameClicked() {
			SetListenersEnabled(false);
			SceneManager.LoadSceneAsync("Game");
		}

		private void HandleBackToMenuClicked() {
			SetListenersEnabled(false);
			SceneManager.LoadSceneAsync("Menu");
		}

		private void HandleQuitClicked() {
			SetListenersEnabled(false);
			Application.Quit();
		}
	}
}