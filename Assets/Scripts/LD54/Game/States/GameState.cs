using LD54.Data;

namespace LD54.Game {
	public abstract class GameState : NiUtils.GameStates.GameState {
		public static GameConfig config { get; set; }
	}
}