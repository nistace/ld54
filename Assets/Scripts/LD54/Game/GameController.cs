using System;
using LD54.Data;
using LD54.Inputs;
using UnityEngine;

namespace LD54.Game {
	public class GameController : MonoBehaviour {
		[SerializeField] private GameConfig config;

		private void Start() {
			GameSessionData.CreateNew(config);
			Storage.current.Init();
			Collector.current.Init();
			PackageSpawner.current.Init();
			OrderManager.Init();
			NiUtils.GameStates.GameState.ChangeState(DefaultGameState.state);
			GameSessionData.current.StartGame();
		}

		private void OnEnable() {
			GameInputs.controls.Enable();
		}

		private void OnDisable() {
			GameInputs.controls.Disable();
		}
	}
}