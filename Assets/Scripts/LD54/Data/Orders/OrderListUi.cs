using System.Collections.Generic;
using System.Linq;
using NiUtils.Extensions;
using UnityEngine;

namespace LD54.Data {
	public class OrderListUi : MonoBehaviour {
		[SerializeField] protected OrderManager _orderManager;
		[SerializeField] protected OrderListItemUi _itemPrefab;
		[SerializeField] protected Transform _itemParent;

		private List<OrderListItemUi> orderedItemsUis { get; } = new List<OrderListItemUi>();
		private Dictionary<Order, OrderListItemUi> uisPerOrder { get; } = new Dictionary<Order, OrderListItemUi>();

		public static Order.Event onSendOrderClicked { get; } = new Order.Event();

		public void Start() {
			orderedItemsUis.AddRange(_itemParent.GetComponentsInChildren<OrderListItemUi>(true));
			while (orderedItemsUis.Count < _orderManager.maxOrderCount) orderedItemsUis.Add(Instantiate(_itemPrefab, _itemParent));
			foreach (var itemUi in orderedItemsUis) {
				itemUi.onSendOrderClicked.AddListenerOnce(onSendOrderClicked.Invoke);
				itemUi.Disable();
			}
			uisPerOrder.Clear();

			_orderManager.onOrderCreated.AddListenerOnce(HandleNewOrder);
			_orderManager.onOrderRemoved.AddListenerOnce(HandleOrderRemoved);

			_orderManager.GetAllActiveOrders().ForEach(HandleNewOrder);
		}

		private void HandleNewOrder(Order order) {
			if (uisPerOrder.ContainsKey(order)) return;
			var slot = orderedItemsUis.FirstOrDefault(t => !t.enabled);
			if (!slot) return;
			slot.Setup(order);
			uisPerOrder.Add(order, slot);
		}

		private void HandleOrderRemoved(Order removedOrder) {
			if (!uisPerOrder.ContainsKey(removedOrder)) return;
			uisPerOrder[removedOrder].Disable();
			uisPerOrder.Remove(removedOrder);
		}
	}
}