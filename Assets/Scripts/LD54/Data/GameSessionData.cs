using NiUtils.Events;
using UnityEngine;

namespace LD54.Data {
	public class GameSessionData {
		public static GameSessionData current { get; private set; }

		public static bool isPlaying => current is { gameStartedTime: >= 0 };
		public static float gameTime => isPlaying ? Time.time - current.gameStartedTime : 0;
		public static GameConfig config { get; private set; }

		public IntEvent onScoreChanged { get; } = new IntEvent();
		public IntEvent onCreditsChanged { get; } = new IntEvent();

		private float gameStartedTime { get; set; } = -1;
		public int score { get; private set; }
		public int credits { get; private set; }

		public static void CreateNew(GameConfig gameConfig) {
			current = new GameSessionData { credits = gameConfig.creditsOnStart };
			config = gameConfig;
		}

		public void StartGame() => gameStartedTime = Time.time;

		public void IncreaseScore(int additionalPoints, int additionalCredits) {
			score += additionalPoints;
			credits += additionalCredits;
			onScoreChanged.Invoke(score);
			onCreditsChanged.Invoke(credits);
		}

		public void PayCredits(int amount) {
			credits -= amount;
			onCreditsChanged.Invoke(credits);
		}
	}
}