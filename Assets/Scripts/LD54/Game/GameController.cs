using LD54.Data;
using LD54.Inputs;
using UnityEngine;

namespace LD54.Game {
	public class GameController : MonoBehaviour {
		[SerializeField] private GameConfig config;

		private void Start() {
			GameState.config = config;
			Storage.current.Build();
			NiUtils.GameStates.GameState.ChangeState(DefaultGameState.state);
		}

		private void OnEnable() {
			GameInputs.controls.Enable();
		}

		private void OnDisable() {
			GameInputs.controls.Disable();
		}
	}
}