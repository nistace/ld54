using System;
using UnityEngine;
using UnityEngine.Events;

namespace LD54.Data {
	[Serializable]
	public class Order {
		public class Event : UnityEvent<Order> { }

		public enum State {
			Active = 0,
			BeingDelivered = 1,
			Expired = 2
		}

		[SerializeField] protected OrderType _type;

		private State state { get; set; }
		public OrderType type => _type;
		public float expirationProgress { get; private set; }
		public bool isActive => state == State.Active;

		public Order() { }

		public Event onExpired { get; } = new Event();

		public Order(OrderType type) {
			_type = type;
			state = State.Active;
		}

		public bool IsReadyToBeDelivered() => state == State.Active && OrderStorageObserver.IsStorageReadyToDeliver(_type);
		public int GetReadyPackageCount(Package packagePrefab) => OrderStorageObserver.CountInstancesInStorage(packagePrefab);
		public int GetRequiredAmount(Package package) => type.GetRequiredAmount(package);

		public void MarkAsBeingDelivered() => state = State.BeingDelivered;

		public void ProgressOnExpiration() {
			if (expirationProgress >= 1) return;
			expirationProgress += Time.deltaTime / _type.expirationTime;
			if (expirationProgress >= 1) {
				state = State.Expired;
				onExpired.Invoke(this);
			}
		}
	}
}