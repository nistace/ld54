using NiUtils.Events;
using UnityEngine;

namespace LD54.Data {
	public class GameSessionData {
		public static GameSessionData current { get; private set; }

		public static bool isPlaying => current is { gameStartedTime: >= 0 };
		public static float gameTime => isPlaying ? (current.gameOver ? current.gameEndedTime : Time.time) - current.gameStartedTime : 0;
		public static GameConfig config { get; private set; }

		public IntEvent onScoreChanged { get; } = new IntEvent();
		public IntEvent onCreditsChanged { get; } = new IntEvent();

		private float gameStartedTime { get; set; } = -1;
		private float gameEndedTime { get; set; } = -1;
		public bool gameOver => gameEndedTime > 0;
		public int score { get; private set; }
		public int credits { get; private set; }

		public static void CreateNew(GameConfig gameConfig) {
			current = new GameSessionData { credits = gameConfig.creditsOnStart };
			config = gameConfig;
		}

		public void StartGame() => gameStartedTime = Time.time;

		public void IncreaseScore(int additionalPoints, int additionalCredits) {
			if (gameOver) return;
			score += additionalPoints;
			credits += additionalCredits;
			onScoreChanged.Invoke(score);
			onCreditsChanged.Invoke(credits);
		}

		public void PayCredits(int amount) {
			credits -= amount;
			onCreditsChanged.Invoke(credits);
		}

		public void Destroy() => current = null;

		public void EndGame() => gameEndedTime = Time.time;
	}
}