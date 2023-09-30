using UnityEngine;
using UnityEngine.Serialization;

namespace LD54.Data {
	[CreateAssetMenu]
	public class PackageInWaterProps : ScriptableObject {
		[SerializeField] protected Vector3 _upForce = Vector3.up;
		[SerializeField] protected ForceMode _forceMode = ForceMode.VelocityChange;
		[SerializeField] protected float _waterLevel = -1;
		[SerializeField] protected float _surfaceHeight = 2;
		[SerializeField] protected float _maxVelocityMagnitudeAtSurface;
		[FormerlySerializedAs("_velocityClampSpeedAtSurface")] [SerializeField] protected float _smoothDampTime = .3f;

		public Vector3 upForce => _upForce;
		public ForceMode forceMode => _forceMode;
		public float waterLevel => _waterLevel;
		public float maxVelocityMagnitudeAtSurface => _maxVelocityMagnitudeAtSurface;
		public float surfaceHeight => _surfaceHeight;
		public float smoothDampTime => _smoothDampTime;
	}
}