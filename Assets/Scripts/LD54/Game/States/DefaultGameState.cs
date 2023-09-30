using System.Collections;
using LD54.Data;
using LD54.Inputs;
using NiUtils.Extensions;
using NiUtils.GameStates;
using UnityEngine;
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
				var cursorRay = StorageCamera.currentCamera.ScreenPointToRay(GameInputs.controls.Player.Aim.ReadValue<Vector2>());
				hitInteractable = Physics.Raycast(cursorRay, out var hitInfo, 50, GameSessionData.current.config.defaultStateHitLayerMask) ? hitInfo.collider.GetComponentInParent<IInteractable>() : null;
				yield return null;
			}
		}

		protected override void SetListenersEnabled(bool enabled) {
			GameInputs.controls.Player.Interact.AddPerformListenerOnce(HandleInteract);
		}

		private void HandleInteract(InputAction.CallbackContext obj) {
			if (hitInteractable == null) return;
			if (hitInteractable is Package package) {
				PlacePackageGameState.state.Init(package);
				ChangeState(PlacePackageGameState.state);
			}
			else if (hitInteractable is ExtensionArea extensionArea) {
				Storage.current.IncreaseWithExtensionArea(extensionArea);
				Storage.current.SetExtensionAreasVisible(true);
			}
		}
	}
}