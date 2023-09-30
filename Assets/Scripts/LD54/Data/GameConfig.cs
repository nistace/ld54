using System;
using System.Collections.Generic;
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
		[SerializeField] protected AnimationCurve _spawnPerSecondCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField] protected Copter _copter = new Copter();
		[SerializeField] protected Package[] _spawnablePackages;

		public LayerMask defaultStateHitLayerMask => _defaultStateHitLayerMask;
		public LayerMask placementStateHitLayerMask => _placementStateHitLayerMask;
		public float placementStatePackageYOffset => _placementStatePackageYOffset;
		public float packageSmoothPosition => _packageSmoothPosition;
		public float packageSmoothRotation => _packageSmoothRotation;
		public float minDelayBetweenPackageRotations => _minDelayBetweenPackageRotations;
		public float placementDelayBeforeInteraction => _placementDelayBeforeInteraction;
		public AnimationCurve spawnPerSecondCurve => _spawnPerSecondCurve;
		public IEnumerable<Package> spawnablePackages => _spawnablePackages;
		public Copter copter => _copter;

		[Serializable] public class Copter {
			[SerializeField] protected AnimationCurve _delaysBetweenTravels;
			[SerializeField] protected Vector3 _origin;
			[SerializeField] protected float _travelDuration = 2;
			[SerializeField] protected AnimationCurve _travelPositionCurve;
			[SerializeField] protected float _magnetOriginHeight = 18;
			[SerializeField] protected float _magnetAnimationDuration;
			[SerializeField] protected AnimationCurve _magnetYPositionCurve;
			[SerializeField] protected AnimationCurve _maxPickUpCountCurve;
			[Range(0, 1), SerializeField] protected float _magnetGrabTimeRatio = .5f;

			public Vector3 origin => _origin;
			public float travelDuration => _travelDuration;
			public float magnetAnimationDuration => _magnetAnimationDuration;
			public float magnetGrabTimeRatio => _magnetGrabTimeRatio;
			public AnimationCurve delaysBetweenTravels => _delaysBetweenTravels;
			public float magnetOriginHeight => _magnetOriginHeight;
			public AnimationCurve magnetYPositionCurve => _magnetYPositionCurve;

			public Vector3 GetTravelPosition(Vector3 targetPosition, float lerp) => Vector3.Lerp(_origin, targetPosition, _travelPositionCurve.Evaluate(lerp));
			public int GetMaxPickUpCount(float gameTime) => Mathf.RoundToInt(_maxPickUpCountCurve.Evaluate(gameTime));
		}
	}
}