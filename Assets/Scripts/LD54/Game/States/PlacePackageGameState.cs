using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD54.Data;
using LD54.Inputs;
using NiUtils.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD54.Game {
	public class PlacePackageGameState : GameState {
		public static PlacePackageGameState state { get; } = new PlacePackageGameState();

		private enum InteractionEffect {
			Forbidden = 0,
			DropInStorage = 1,
			DropOutsideOfStorage = 2
		}

		private Package package { get; set; }
		private Vector3 initialPosition { get; set; }
		private ISet<Vector2Int> initialPositionOccupiedStorageCells { get; set; } = new HashSet<Vector2Int>();
		private Vector3 packageDesiredPosition { get; set; }
		private Vector3 packageDesiredForward { get; set; }
		private Vector3 positionVelocity;
		private Vector3 rotationVelocity;
		private float stateStartTime { get; set; }
		private float lastRotationTime { get; set; }
		private ISet<Vector2Int> hoveredCellCoordinates { get; set; } = new HashSet<Vector2Int>();
		private InteractionEffect interactionEffect { get; set; }

		private PlacePackageGameState() { }

		public void Init(Package package) {
			this.package = package;
			initialPosition = package.transform.position;
		}

		protected override void Enable() {
			if (!package) return;
			initialPositionOccupiedStorageCells = Storage.current.GetAllCellsContainingPackage(package);
			Storage.current.RemovePackage(package);
			package.rigidbody.isKinematic = true;
			packageDesiredPosition = package.transform.position.With(y: config.placementStatePackageYOffset);
			packageDesiredForward = SnapForwardToAllowedAngle(package.transform.forward);
			rotationVelocity = Vector3.zero;
			positionVelocity = Vector3.zero;
			stateStartTime = Time.time;
		}

		private static Vector3 SnapForwardToAllowedAngle(Vector3 forward) {
			if (Mathf.Approximately(Mathf.Abs(forward.y), 1)) return Vector3.forward;
			if (Mathf.Approximately(Mathf.Abs(forward.x), 1) || Mathf.Approximately(Mathf.Abs(forward.z), 1)) return forward;
			if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z)) {
				return forward.x > 0 ? Vector3.right : Vector3.left;
			}
			return forward.z > 0 ? Vector3.forward : Vector3.back;
		}

		protected override void Disable() {
			if (package) package.rigidbody.isKinematic = false;
			Storage.current.UnmarkAllCells();
		}

		protected override IEnumerator Continue() {
			while (currentState == this) {
				interactionEffect = InteractionEffect.Forbidden;
				if (IsStartDelayPassed()) {
					var cursorRay = StorageCamera.currentCamera.ScreenPointToRay(GameInputs.controls.Player.Aim.ReadValue<Vector2>());
					if (Physics.Raycast(cursorRay, out var hitInfo, 50, config.placementStateHitLayerMask)) {
						packageDesiredPosition = Storage.SnapShapeCenterToGrid(hitInfo.point, package.shape) + Vector3.up * config.placementStatePackageYOffset;
						var newlyHoveredCellCoordinates = Storage.current.GetHoveredCellCoordinates(packageDesiredPosition, package.shape, out var anyInGrid).ToSet();
						Storage.current.UnmarkAllCells();
						if (!anyInGrid) {
							interactionEffect = InteractionEffect.DropOutsideOfStorage;
						}
						else {
							var hasUnavailableCells = false;
							foreach (var hoveredCell in newlyHoveredCellCoordinates) {
								var cellIsAvailable = Storage.current.IsAvailable(hoveredCell);
								hasUnavailableCells |= !cellIsAvailable;
								Storage.current.MarkCell(hoveredCell, cellIsAvailable ? StorageCellData.MaterialType.ValidPosition : StorageCellData.MaterialType.InvalidPosition);
							}
							hoveredCellCoordinates = newlyHoveredCellCoordinates;
							interactionEffect = hasUnavailableCells ? InteractionEffect.Forbidden : InteractionEffect.DropInStorage;
						}
					}
				}
				package.transform.position = Vector3.SmoothDamp(package.transform.position, packageDesiredPosition, ref positionVelocity, config.packageSmoothPosition);
				package.transform.forward = Vector3.SmoothDamp(package.transform.forward, packageDesiredForward, ref rotationVelocity, config.packageSmoothRotation);
				yield return null;
			}
		}

		private bool IsStartDelayPassed() => stateStartTime + config.placementDelayBeforeInteraction < Time.time;

		protected override void SetListenersEnabled(bool enabled) {
			GameInputs.controls.Player.Interact.SetPerformListenerOnce(HandlePlace, enabled);
			GameInputs.controls.Player.RotatePackage.SetAnyListenerOnce(HandleRotation, enabled);
			GameInputs.controls.Player.Cancel.SetPerformListenerOnce(HandleCancel, enabled);
		}

		private void HandleRotation(InputAction.CallbackContext obj) {
			if (!IsStartDelayPassed()) return;
			if (lastRotationTime + config.minDelayBetweenPackageRotations > Time.time) return;
			packageDesiredForward = Quaternion.Euler(0, -Mathf.RoundToInt(obj.ReadValue<float>()) * 90, 0) * packageDesiredForward;
			lastRotationTime = Time.time;
		}

		private void HandleCancel(InputAction.CallbackContext obj) {
			package.transform.position = initialPosition;
			ChangeState(DefaultGameState.state);
		}

		private void HandlePlace(InputAction.CallbackContext obj) {
			if (interactionEffect == InteractionEffect.Forbidden) {
				Debug.Log("Nope");
				return;
			}
			if (interactionEffect == InteractionEffect.DropInStorage) {
				Storage.current.SetPackage(hoveredCellCoordinates, package);
			}
			ChangeState(DefaultGameState.state);
		}
	}
}