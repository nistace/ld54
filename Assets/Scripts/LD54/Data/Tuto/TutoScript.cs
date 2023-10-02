using System;
using Unity.VisualScripting;
using UnityEngine;

namespace LD54.Data {
	public class TutoScript : MonoBehaviour {
		public static TutoScript current { get; set; }

		[SerializeField] protected Step[] _firstSteps;
		[SerializeField] protected Step[] _deliverySteps;
		[SerializeField] protected Step[] _collectSteps;

		public Step[] firstSteps => _firstSteps;
		public Step[] deliverySteps => _deliverySteps;
		public Step[] collectSteps => _collectSteps;

		private void Awake() {
			current = this;
		}

		[Serializable] public class Step {
			public enum Arrow {
				None = 0,
				ObjectInScene = 1,
				ObjectInUi = 3,
				SpawnedPackage = 2
			}

			[SerializeField] protected Arrow _arrow;
			[SerializeField] protected string _text;
			[SerializeField] protected Transform _arrowTarget;
			[SerializeField] protected bool _forceCreateNewOrder;
			[SerializeField] protected bool _showExtensionSteps;

			public Arrow arrow => _arrow;
			public string text => _text;
			public Transform arrowTarget => _arrowTarget;
			public bool forceCreateNewOrder => _forceCreateNewOrder;
			public bool showExtensionSteps => _showExtensionSteps;
		}
	}
}