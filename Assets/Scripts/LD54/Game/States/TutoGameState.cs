using System;
using System.Collections;
using System.Collections.Generic;
using LD54.Data;
using NiUtils.Extensions;
using NiUtils.GameStates;
using NiUtils.StaticUtils;
using UnityEngine;
using CoroutineRunner = NiUtils.Coroutines.CoroutineRunner;

namespace LD54.Game {
	public class TutoGameState : GameState {
		public static TutoGameState state { get; } = new TutoGameState();
		private TutoScript script => TutoScript.current;
		private IReadOnlyList<TutoScript.Step> currentStepList { get; set; }
		private int scriptStepIndex { get; set; }
		private Order tutoOrder { get; set; }

		private TutoGameState() { }

		protected override void Enable() {
			Storage.current.SetExtensionAreasVisible(false);
			scriptStepIndex = 0;
			currentStepList = GetCurrentStepList();
			DisplayCurrentStep();
		}

		protected override void Disable() {
			TutoUi.current.Hide();
			if (GameSessionData.current.tutoStep == GameSessionData.TutoStep.First) {
				OrderStorageObserver.onStorageStateChanged.AddListenerOnce(CheckToEnableSecondStep);
			}
			else if (GameSessionData.current.tutoStep == GameSessionData.TutoStep.Second) {
				GameSessionData.current.onScoreChanged.AddListenerOnce(HandleStartThirdStep);
			}
			else if (GameSessionData.current.tutoStep == GameSessionData.TutoStep.Third) {
				OrderStorageObserver.onStorageStateChanged.RemoveListener(CheckToEnableSecondStep);
				GameSessionData.current.onScoreChanged.RemoveListener(HandleStartThirdStep);
				GameSessionData.current.tutoStep = GameSessionData.TutoStep.Done;
				GameSessionData.current.StartGame();
			}
		}

		private void HandleStartThirdStep(int _) {
			GameSessionData.current.tutoStep = GameSessionData.TutoStep.Third;
			CoroutineRunner.Run(TakeStateNextFrame());
		}

		private void DisplayCurrentStep() {
			var step = currentStepList[scriptStepIndex];
			if (step.forceCreateNewOrder) tutoOrder = OrderManager.current.ForceCreateNewOrder();
			Storage.current.SetExtensionAreasVisible(step.showExtensionSteps);
			TutoUi.current.Show(step.text, step.arrow != TutoScript.Step.Arrow.None, GetArrowTargetScreenPosition(step));
		}

		private static Vector3 GetArrowTargetScreenPosition(TutoScript.Step step) => step.arrow switch {
			TutoScript.Step.Arrow.None => Vector3.zero,
			TutoScript.Step.Arrow.ObjectInScene => step.arrowTarget ? CameraUtils.main.WorldToScreenPoint(step.arrowTarget.position) : Vector3.zero,
			TutoScript.Step.Arrow.ObjectInUi => step.arrowTarget ? step.arrowTarget.position : Vector3.zero,
			TutoScript.Step.Arrow.SpawnedPackage => CameraUtils.main.WorldToScreenPoint(PackageSpawner.current.GetAnyActivePackage()?.transform.position ?? Vector3.zero),
			_ => throw new ArgumentOutOfRangeException()
		};

		private IReadOnlyList<TutoScript.Step> GetCurrentStepList() => GameSessionData.current.tutoStep switch {
			GameSessionData.TutoStep.First => script.firstSteps,
			GameSessionData.TutoStep.Second => script.deliverySteps,
			GameSessionData.TutoStep.Third => script.collectSteps,
			_ => throw new ArgumentOutOfRangeException()
		};

		private void CheckToEnableSecondStep() {
			if (tutoOrder.IsReadyToBeDelivered()) {
				GameSessionData.current.tutoStep = GameSessionData.TutoStep.Second;
				CoroutineRunner.Run(TakeStateNextFrame());
			}
		}

		private IEnumerator TakeStateNextFrame() {
			yield return null;
			ChangeState(this);
		}

		protected override IEnumerator Continue() {
			yield break;
		}

		protected override void SetListenersEnabled(bool enabled) {
			TutoUi.current.onContinueClicked.AddListenerOnce(HandleContinue);
		}

		private void HandleContinue() {
			scriptStepIndex++;
			if (scriptStepIndex >= GetCurrentStepList().Count) {
				ChangeState(DefaultGameState.state);
			}
			else {
				DisplayCurrentStep();
			}
		}
	}
}