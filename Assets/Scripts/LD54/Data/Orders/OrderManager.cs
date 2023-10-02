using System.Collections.Generic;
using System.Linq;
using NiUtils.Extensions;
using UnityEngine;

namespace LD54.Data {
	public class OrderManager : MonoBehaviour {
		public static OrderManager current { get; private set; }

		[SerializeField] protected int _maxOrderCount = 6;

		public int maxOrderCount => _maxOrderCount;
		private HashSet<OrderType> possibleOrderTypes { get; } = new HashSet<OrderType>();
		private List<OrderType> nextOrderTypes { get; } = new List<OrderType>();
		private List<Order> activeOrders { get; } = new List<Order>();
		private Dictionary<Order, List<Package>> ordersBeingDelivered { get; } = new Dictionary<Order, List<Package>>();

		public Order.Event onOrderCreated { get; } = new Order.Event();
		public Order.Event onOrderRemoved { get; } = new Order.Event();

		private float nextOrderProgress { get; set; } = 1;

		private void Awake() {
			current = this;
		}

		public static void Init() {
			OrderStorageObserver.Init(GameSessionData.config.order.GetAllTypes().SelectMany(t => t.amounts).Select(t => t.package));
		}

		private void Update() {
			UpdateOrdersBeingDelivered();
			if (!GameSessionData.isPlaying || GameSessionData.current.gameOver) return;
			UpdateNextOrder();
			UpdateExpirationTimes();
		}

		private void UpdateExpirationTimes() {
			foreach (var order in activeOrders) {
				order.ProgressOnExpiration();
			}
		}

		private void UpdateOrdersBeingDelivered() {
			if (ordersBeingDelivered.Count == 0) return;
			foreach (var order in ordersBeingDelivered.Keys.ToArray()) {
				for (var i = 0; i < ordersBeingDelivered[order].Count; ++i) {
					if (ordersBeingDelivered[order][i].state == Package.State.Delivered) {
						ordersBeingDelivered[order].RemoveAt(i);
						i--;
					}
				}
				if (ordersBeingDelivered[order].Count == 0) {
					GameSessionData.current.IncreaseScore(order.type.points, order.type.credits);
					ordersBeingDelivered.Remove(order);
					onOrderRemoved.Invoke(order);
				}
			}
		}

		private void UpdateNextOrder() {
			nextOrderProgress += GameSessionData.config.order.GetNewOrderProgress(GameSessionData.gameTime);
			while (activeOrders.Count + ordersBeingDelivered.Count < _maxOrderCount && (activeOrders.Count == 0 || nextOrderProgress > 1)) {
				CreateNewOrder();
				nextOrderProgress = 0;
			}
		}

		private Order CreateNewOrder() {
			if (nextOrderTypes.Count == 0) {
				possibleOrderTypes.AddAll(GameSessionData.config.order.GetUnlockedTypesAfterTime(GameSessionData.gameTime));
				nextOrderTypes.AddRange(possibleOrderTypes.OrderBy(_ => Random.value));
			}
			var randomOrderType = nextOrderTypes[0];
			nextOrderTypes.RemoveAt(0);
			var newOrder = new Order(randomOrderType, 1 + activeOrders.Count(t => t.isExpired) * .5f);
			activeOrders.Add(newOrder);
			PackageSpawner.current.SpawnForOrder(newOrder.type);
			onOrderCreated.Invoke(newOrder);
			return newOrder;
		}

		public IReadOnlyList<Order> GetAllActiveOrders() => activeOrders;

		public void SendOrder(Order order) {
			if (order == null) return;
			if (!OrderStorageObserver.TryGetListOfPackagesToDeliver(order.type, out var packagesToCollect)) return;
			Collector.current.Collect(packagesToCollect);
			activeOrders.Remove(order);
			order.MarkAsBeingDelivered();
			ordersBeingDelivered.Add(order, packagesToCollect.ToList());
		}

		public bool IsFullWithExpired() => activeOrders.Count == _maxOrderCount && activeOrders.All(t => t.isExpired);

		public Order ForceCreateNewOrder() => CreateNewOrder();
	}
}