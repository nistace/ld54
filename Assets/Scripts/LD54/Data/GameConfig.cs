using UnityEngine;

namespace LD54.Data {
	[CreateAssetMenu]
	public class GameConfig : ScriptableObject {
		[SerializeField] protected LayerMask _defaultStateHitLayerMask;
		[SerializeField] protected LayerMask _placementStateHitLayerMask;
		[SerializeField] protected float _placementStatePackageYOffset = 1.5f;
		[SerializeField] protected float _packageSmoothPosition = .5f;
		[SerializeField] protected float _packageSmoothRotation = .5f;
		[SerializeField] protected float _minDelayBetweenPackageRotations = .1f;
		[SerializeField] protected float _placementDelayBeforeInteraction = .1f;

		public LayerMask defaultStateHitLayerMask => _defaultStateHitLayerMask;
		public LayerMask placementStateHitLayerMask => _placementStateHitLayerMask;
		public float placementStatePackageYOffset => _placementStatePackageYOffset;
		public float packageSmoothPosition => _packageSmoothPosition;
		public float packageSmoothRotation => _packageSmoothRotation;
		public float minDelayBetweenPackageRotations => _minDelayBetweenPackageRotations;
		public float placementDelayBeforeInteraction => _placementDelayBeforeInteraction;
	}
}