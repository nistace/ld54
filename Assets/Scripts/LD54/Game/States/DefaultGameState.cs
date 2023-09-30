using System.Collections;
using LD54.Data;
using LD54.Inputs;
using NiUtils.Extensions;
using NiUtils.StaticUtils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD54.Game {
	public class DefaultGameState : GameState {
		public static DefaultGameState state { get; } = new DefaultGameState();

		private DefaultGameState() { }

		private Transform currentlyHitObject { get; set; }

		protected override void Enable() { }

		protected override void Disable() { }

		protected override IEnumerator Continue() {
			while (currentState == this) {
				var cursorRay = StorageCamera.currentCamera.ScreenPointToRay(GameInputs.controls.Player.Aim.ReadValue<Vector2>());
				var newlyHitObject = Physics.Raycast(cursorRay, out var hitInfo, 20, config.defaultStateHitLayerMask) ? hitInfo.collider.transform.root : null;
				if (newlyHitObject != currentlyHitObject) {
					currentlyHitObject = newlyHitObject;
				}
				yield return null;
			}
		}

		protected override void SetListenersEnabled(bool enabled) {
			GameInputs.controls.Player.Interact.AddPerformListenerOnce(HandleInteract);
		}

		private void HandleInteract(InputAction.CallbackContext obj) {
			if (!currentlyHitObject) return;
			if (currentlyHitObject.TryGetComponent<Package>(out var package)) {
				PlacePackageGameState.state.Init(package);
				ChangeState(PlacePackageGameState.state);
			}
		}
	}
}