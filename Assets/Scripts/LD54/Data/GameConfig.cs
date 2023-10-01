using System;
using System.Collections.Generic;
using System.Linq;
using NiUtils.Types;
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
		[SerializeField] protected int _creditsOnStart = 2;
		[SerializeField] protected Order _order = new Order();
		[SerializeField] protected Collector _collector = new Collector();

		public LayerMask defaultStateHitLayerMask => _defaultStateHitLayerMask;
		public LayerMask placementStateHitLayerMask => _placementStateHitLayerMask;
		public float placementStatePackageYOffset => _placementStatePackageYOffset;
		public float packageSmoothPosition => _packageSmoothPosition;
		public float packageSmoothRotation => _packageSmoothRotation;
		public float minDelayBetweenPackageRotations => _minDelayBetweenPackageRotations;
		public float placementDelayBeforeInteraction => _placementDelayBeforeInteraction;
		public int creditsOnStart => _creditsOnStart;
		public Collector collector => _collector;
		public Order order => _order;

		[Serializable] public class Order {
			[SerializeField] protected AnimationCurve _newOrderCurve = AnimationCurve.Linear(0, 0, 1, 1);
			[SerializeField] protected Color _uiCounterValidColor = Color.green;
			[SerializeField] protected Color _uiCounterInvalidColor = Color.gray;
			[SerializeField] protected Color _uiExpiredOrderColor = Color.gray;
			[SerializeField] protected FloatRange _uiExpiredMoveDownOffset = new FloatRange(.5f, 1);
			[SerializeField] protected FloatRange _uiExpiredRotateMaxAngle = new FloatRange(15, 30);
			[SerializeField] protected float _uiExpiredAnimationDuration = .5f;
			[SerializeField] protected UnlockedOrderType[] possibleOrders;

			public Color uiCounterValidColor => _uiCounterValidColor;
			public Color uiCounterInvalidColor => _uiCounterInvalidColor;
			public Color uiExpiredOrderColor => _uiExpiredOrderColor;
			public FloatRange uiExpiredMoveDownOffset => _uiExpiredMoveDownOffset;
			public FloatRange uiExpiredRotateMaxAngle => _uiExpiredRotateMaxAngle;
			public float uiExpiredAnimationDuration => _uiExpiredAnimationDuration;

			public float GetNewOrderProgress(float time) => _newOrderCurve.Evaluate(time) * Time.deltaTime;
			public IEnumerable<OrderType> GetUnlockedTypesAfterTime(float time) => possibleOrders.Where(t => t.time <= time).Select(t => t.type);
			public IEnumerable<OrderType> GetAllTypes() => possibleOrders.Select(t => t.type);

			[Serializable] public class UnlockedOrderType {
				[SerializeField] protected OrderType _type;
				[SerializeField] protected float _time;

				public OrderType type => _type;
				public float time => _time;
			}
		}

		[Serializable] public class Collector {
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