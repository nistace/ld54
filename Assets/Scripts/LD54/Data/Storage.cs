using System.Collections.Generic;
using System.Linq;
using NiUtils.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace LD54.Data {
	public class Storage : MonoBehaviour {
		public static Storage current { get; private set; }

		[SerializeField] protected int _width = 5;
		[SerializeField] protected int _length = 5;
		[SerializeField] protected Transform _ground;
		[SerializeField] protected Transform _cellParent;
		[SerializeField] protected StorageCell _cellPrefab;
		[SerializeField] protected ExtensionArea _widthExtensionArea;
		[SerializeField] protected ExtensionArea _lengthExtensionArea;
		[SerializeField] protected int _maxWidth = 8;
		[SerializeField] protected int _maxLength = 8;

		public Vector3 center => new Vector3((_width - 1) * .5f, 0, (_length - 1) * .5f);
		public int size => Mathf.Max(_width, _length);
		private Dictionary<Vector2Int, StorageCell> cells { get; } = new Dictionary<Vector2Int, StorageCell>();
		private List<StorageCell> hiddenFakeCells { get; } = new List<StorageCell>();
		private Dictionary<Vector2Int, StorageCell> visibleFakeCells { get; } = new Dictionary<Vector2Int, StorageCell>();
		private List<Package> packagesReadyToDeliver { get; } = new List<Package>();
		public bool isFull => cells.Values.All(t => t.package);

		private void Awake() {
			current = this;
		}

		[ContextMenu("Build")]
		public void Build() {
			_cellParent.ClearChildren();

			for (var x = 0; x < _width; ++x)
			for (var y = 0; y < _length; ++y) {
				CreateCell(x, y);
			}
			RefreshExtensionAreas();
		}

		private void IncreaseWidth() {
			_width++;
			for (var y = 0; y < _length; ++y) CreateCell(_width - 1, y);
			RefreshGround();
			RefreshExtensionAreas();
		}

		private void IncreaseLength() {
			_length++;
			for (var x = 0; x < _width; ++x) CreateCell(x, _length - 1);
			RefreshGround();
			RefreshExtensionAreas();
		}

		private void RefreshGround() {
			_ground.localScale = new Vector3(_width, 8, _length);
			_ground.position = new Vector3((_width - 1) * .5f, -4, (_length - 1) * .5f);
		}

		private void RefreshExtensionAreas() {
			_widthExtensionArea.RefreshOnWidth(_width, _length);
			_lengthExtensionArea.RefreshOnLength(_width, _length);
		}

		public void SetExtensionAreasVisible(bool visible) {
			_widthExtensionArea.gameObject.SetActive(visible && _width < _maxWidth);
			_lengthExtensionArea.gameObject.SetActive(visible && _length < _maxLength);
		}

		public void IncreaseWithExtensionArea(ExtensionArea extensionArea) {
			if (extensionArea == _widthExtensionArea) IncreaseWidth();
			else if (extensionArea == _lengthExtensionArea) IncreaseLength();
		}

		private StorageCell CreateCell(int x, int y, bool fake = false) {
			var coordinates = new Vector2Int(x, y);
			if (cells.ContainsKey(coordinates)) return cells[coordinates];
			var newCell = Instantiate(_cellPrefab, CoordinatesToWorldPosition(x, y), Quaternion.identity, _cellParent);
			newCell.SetMaterial(StorageCellData.MaterialType.Default);
			if (!fake) cells.Add(coordinates, newCell);
			return newCell;
		}

		private static Vector2Int WorldPositionToCoordinates(Vector3 worldPosition) => new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));

		public static Vector3 CoordinatesToShapeCenterWorldPosition(Vector2Int coordinates, PackageShape shape) {
			return CoordinatesToWorldPosition(coordinates.x, coordinates.y) + new Vector3((shape.width + 1) % 2 * .5f, 0, (shape.length + 1) % 2 * .5f);
		}

		private static Vector3 CoordinatesToWorldPosition(Vector2Int coordinates) => CoordinatesToWorldPosition(coordinates.x, coordinates.y);
		private static Vector3 CoordinatesToWorldPosition(int x, int y) => new Vector3(x, 0, y);

		public static Vector3 SnapShapeCenterToGrid(Vector3 position, PackageShape shape) {
			var x = shape.width % 2 == 1 ? Mathf.RoundToInt(position.x) : Mathf.RoundToInt(position.x - .5f) + .5f;
			var z = shape.length % 2 == 1 ? Mathf.RoundToInt(position.z) : Mathf.RoundToInt(position.z - .5f) + .5f;
			return new Vector3(x, 0, z);
		}

		public IEnumerable<Vector2Int> GetHoveredCellCoordinates(Vector3 centerWorldPosition, PackageShape packageShape, out bool anyInGrid) {
			var result = new HashSet<Vector2Int>();
			anyInGrid = false;
			var zeroCoordinates = WorldPositionToCoordinates(centerWorldPosition - new Vector3((packageShape.width - 1) * .5f, 0, (packageShape.length - 1) * .5f));
			for (var x = 0; x < packageShape.width; ++x)
			for (var y = 0; y < packageShape.length; ++y) {
				if (packageShape[x, y]) {
					var coordinates = new Vector2Int(zeroCoordinates.x + x, zeroCoordinates.y + y);
					anyInGrid |= IsInGrid(coordinates);
					result.Add(coordinates);
				}
			}
			return result;
		}

		public void UnmarkAllCells() {
			cells.Keys.ForEach(UnmarkCell);
			while (visibleFakeCells.Count > 0) UnmarkCell(visibleFakeCells.First().Key);
		}

		private void UnmarkCell(Vector2Int coordinates) {
			if (cells.ContainsKey(coordinates)) {
				cells[coordinates].SetMaterial(StorageCellData.MaterialType.Default);
			}
			else if (visibleFakeCells.ContainsKey(coordinates)) {
				var fakeCell = visibleFakeCells.First();
				visibleFakeCells.Remove(fakeCell.Key);
				hiddenFakeCells.Add(fakeCell.Value);
				fakeCell.Value.gameObject.SetActive(false);
			}
		}

		public void MarkCell(Vector2Int coordinates, StorageCellData.MaterialType type) {
			if (cells.ContainsKey(coordinates)) {
				cells[coordinates].SetMaterial(type);
			}
			else if (!visibleFakeCells.ContainsKey(coordinates)) {
				if (hiddenFakeCells.Count == 0) hiddenFakeCells.Add(CreateCell(coordinates.x, coordinates.y, true));
				var cell = hiddenFakeCells[0];
				hiddenFakeCells.RemoveAt(0);
				cell.SetMaterial(StorageCellData.MaterialType.InvalidPosition);
				cell.transform.position = CoordinatesToWorldPosition(coordinates);
				cell.gameObject.SetActive(true);
				visibleFakeCells.Add(coordinates, cell);
			}
		}

		private bool IsInGrid(Vector2Int coordinates) => cells.ContainsKey(coordinates);

		public bool IsAvailable(Vector2Int coordinates) => IsInGrid(coordinates) && !cells[coordinates].package;

		public void RemovePackage(Package package) {
			packagesReadyToDeliver.Remove(package);
			foreach (var cell in cells.Values.Where(cell => cell.package == package)) {
				cell.package = null;
			}
		}

		public void AddPackage(IEnumerable<Vector2Int> coordinatesToOccupy, Package package) {
			if (!packagesReadyToDeliver.Contains(package)) packagesReadyToDeliver.Add(package);
			foreach (var coordinates in coordinatesToOccupy) {
				Assert.IsTrue(cells.ContainsKey(coordinates));
				Assert.IsFalse(cells[coordinates].package);
				cells[coordinates].package = package;
			}
		}

		public ISet<Vector2Int> GetAllCellsContainingPackage(Package package) => cells.Where(t => t.Value.package == package).Select(t => t.Key).ToSet();

		public bool TryGetPackageToBeDelivered(out Package packageToDeliver) {
			packageToDeliver = null;
			var biggestPackage = packagesReadyToDeliver.MaxOrDefault(t => t.shape.shapeCellCount);
			for (var i = 0; !packageToDeliver && i < packagesReadyToDeliver.Count; ++i) {
				if (i == packagesReadyToDeliver.Count - 1 || Random.value < .5f + .3f * packagesReadyToDeliver[i].shapeCellSize / biggestPackage) {
					packageToDeliver = packagesReadyToDeliver[i];
				}
			}
			return packageToDeliver;
		}
	}
}