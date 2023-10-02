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
			NiUtils.GameStates.GameState.ChangeState(GameSessionData.tutoToBePlayed ? TutoGameState.state : DefaultGameState.state);
		}

		private void OnEnable() {
			GameInputs.controls.Enable();
		}

		private void OnDisable() {
			GameInputs.controls.Disable();
		}
	}
}