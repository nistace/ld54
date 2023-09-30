using System;
using UnityEngine;

namespace LD54.Data {
	[CreateAssetMenu]
	public class StorageCellData : ScriptableObject {
		public enum MaterialType {
			Default = 0,
			ValidPosition = 1,
			InvalidPosition = 2,
		}

		[SerializeField] protected Material _defaultPositionMaterial;
		[SerializeField] protected Material _validPositionMaterial;
		[SerializeField] protected Material _invalidPositionMaterial;

		public Material GetMaterial(MaterialType type) => type switch {
			MaterialType.Default => _defaultPositionMaterial,
			MaterialType.ValidPosition => _validPositionMaterial,
			MaterialType.InvalidPosition => _invalidPositionMaterial,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
}