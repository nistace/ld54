using UnityEngine;

namespace LD54.Data {
	public class GameSessionData {
		public static GameSessionData current { get; private set; }

		public static bool isPlaying => current is { gameStartedTime: >= 0 };
		public static float gameTime => isPlaying ? Time.time - current.gameStartedTime : 0;

		public float gameStartedTime { get; set; } = -1;
		public GameConfig config { get; private set; }

		public static void CreateNew(GameConfig gameConfig) {
			current = new GameSessionData { config = gameConfig };
		}

		public void StartGame() => gameStartedTime = Time.time;
	}
}