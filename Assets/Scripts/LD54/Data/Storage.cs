using System.Collections.Generic;
using NiUtils.Extensions;
using UnityEngine;

namespace LD54.Data {
	public class Storage : MonoBehaviour {
		public static Storage current { get; private set; }

		[SerializeField] protected int width = 5;
		[SerializeField] protected int length = 5;
		[SerializeField] protected StorageCell cellPrefab;

		public Vector3 center => new Vector3((width - 1) * .5f, 0, (length - 1) * .5f);
		public int size => Mathf.Max(width, length);
		private Dictionary<Vector2Int, StorageCell> cells { get; } = new Dictionary<Vector2Int, StorageCell>();

		private void Awake() {
			current = this;
		}

		[ContextMenu("Build")]
		public void Build() {
			transform.ClearChildren();

			for (var x = 0; x < width; ++x)
			for (var y = 0; y < length; ++y) {
				CreateCell(x, y);
			}
		}

		public void IncreaseWidth() {
			width++;
			for (var y = 0; y < length; ++y) CreateCell(width - 1, y);
		}

		public void IncreaseLength() {
			length++;
			for (var x = 0; x < length; ++x) CreateCell(x, length - 1);
		}

		private void CreateCell(int x, int y) {
			var coordinates = new Vector2Int(x, y);
			if (cells.ContainsKey(coordinates)) return;
			cells.Add(coordinates, Instantiate(cellPrefab, CoordinatesToWorldPosition(x, y), Quaternion.identity, transform));
			cells[coordinates].SetMaterial(StorageCellData.MaterialType.Default);
		}

		public Vector2Int WorldPositionToCoordinates(Vector3 worldPosition) => new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
		public Vector3 CoordinatesToWorldPosition(Vector2Int coordinates) => CoordinatesToWorldPosition(coordinates.x, coordinates.y);
		public Vector3 CoordinatesToWorldPosition(int x, int y) => new Vector3(x, 0, y);

		public bool IsCoordinatesInStorageArea(Vector2Int coordinates) {
			if (coordinates.x < 0) return false;
			if (coordinates.y < 0) return false;
			if (coordinates.x >= width) return false;
			if (coordinates.y >= length) return false;
			return true;
		}

		public void UnmarkAllCells() => cells.Values.ForEach(t => t.SetMaterial(StorageCellData.MaterialType.Default));
		public void MarkCell(Vector2Int coordinates, StorageCellData.MaterialType type) => cells[coordinates].SetMaterial(type);
	}
}