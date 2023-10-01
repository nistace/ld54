using System.Collections;
using LD54.Data;
using LD54.Inputs;
using NiUtils.Extensions;
using NiUtils.GameStates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LD54.Game {
	public class DefaultGameState : GameState {
		public static DefaultGameState state { get; } = new DefaultGameState();

		private DefaultGameState() { }

		private IInteractable hitInteractable { get; set; }

		protected override void Enable() {
			Storage.current.SetExtensionAreasVisible(true);
		}

		protected override void Disable() {
			Storage.current.SetExtensionAreasVisible(false);
		}

		protected override IEnumerator Continue() {
			while (currentState == this) {
				if (EventSystem.current.IsPointerOverGameObject()) {
					hitInteractable = null;
				}
				else {
					var cursorRay = StorageCamera.currentCamera.ScreenPointToRay(GameInputs.controls.Player.Aim.ReadValue<Vector2>());
					hitInteractable = Physics.Raycast(cursorRay, out var hitInfo, 50, GameSessionData.config.defaultStateHitLayerMask) ? hitInfo.collider.GetComponentInParent<IInteractable>() : null;
				}

				yield return null;
			}
		}

		protected override void SetListenersEnabled(bool enabled) {
			OrderListUi.onSendOrderClicked.SetListenerActive(OrderManager.current.SendOrder, enabled);
			GameInputs.controls.Player.Interact.SetPerformListenerOnce(HandleInteract, enabled);
			GameSessionData.current.onCreditsChanged.SetListenerActive(RefreshExtensionAreas, enabled);
		}

		private static void RefreshExtensionAreas(int arg0) => Storage.current.SetExtensionAreasVisible(true);

		private void HandleInteract(InputAction.CallbackContext obj) {
			if (hitInteractable == null) return;
			if (hitInteractable is PackageReserve packageReserve) {
				PlacePackageGameState.state.Init(packageReserve.Spawn());
				ChangeState(PlacePackageGameState.state);
			}
			else if (hitInteractable is Package { state: Package.State.Default } package) {
				PlacePackageGameState.state.Init(package);
				ChangeState(PlacePackageGameState.state);
			}
			else if (hitInteractable is ExtensionArea extensionArea) {
				GameSessionData.current.PayCredits(extensionArea.cost);
				Storage.current.IncreaseWithExtensionArea(extensionArea);
				Storage.current.SetExtensionAreasVisible(true);
			}
		}
	}
}