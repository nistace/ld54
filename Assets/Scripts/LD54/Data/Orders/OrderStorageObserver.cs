using System.Collections.Generic;
using System.Linq;
using NiUtils.Extensions;
using UnityEngine.Events;

namespace LD54.Data {
	public static class OrderStorageObserver {
		private static Dictionary<Package, HashSet<Package>> relevantPackagesPerPrefab { get; } = new Dictionary<Package, HashSet<Package>>();

		public static UnityEvent onStorageStateChanged { get; } = new UnityEvent();

		public static void Init(IEnumerable<Package> allPackagePrefabs) {
			relevantPackagesPerPrefab.SetAll(allPackagePrefabs.Distinct().Select(t => new KeyValuePair<Package, HashSet<Package>>(t, new HashSet<Package>())));
			foreach (var packageAlreadyInStorage in Storage.current.GetPackagesReadyToDeliver()) {
				HandlePackageAddedInStorage(packageAlreadyInStorage);
			}
			Storage.current.onPackageAdded.AddListenerOnce(HandlePackageAddedInStorage);
			Storage.current.onPackageRemoved.AddListenerOnce(HandlePackageRemovedInStorage);
		}

		private static void HandlePackageAddedInStorage(Package addedPackage) {
			if (addedPackage.state != Package.State.Default) return;
			var prefab = relevantPackagesPerPrefab.Keys.FirstOrDefault(t => t.HasSameTypeAs(addedPackage));
			if (!prefab) return;
			var changed = relevantPackagesPerPrefab[prefab].Add(addedPackage);
			addedPackage.onDelivered.AddListenerOnce(HandlePackageRemovedInStorage);
			addedPackage.onLocked.AddListenerOnce(HandlePackageRemovedInStorage);

			if (changed) onStorageStateChanged.Invoke();
		}

		private static void HandlePackageRemovedInStorage(Package removedPackage) {
			var changed = relevantPackagesPerPrefab.Values.Aggregate(false, (current, packageSet) => current | packageSet.Remove(removedPackage));
			removedPackage.onDelivered.AddListenerOnce(HandlePackageRemovedInStorage);
			removedPackage.onLocked.AddListenerOnce(HandlePackageRemovedInStorage);
			if (changed) onStorageStateChanged.Invoke();
		}

		public static bool IsStorageReadyToDeliver(OrderType orderType) {
			foreach (var amount in orderType.amounts) {
				if (!relevantPackagesPerPrefab.ContainsKey(amount.package)) return false;
				if (relevantPackagesPerPrefab[amount.package].Count < amount.amount) return false;
			}
			return true;
		}

		public static bool TryGetListOfPackagesToDeliver(OrderType orderType, out HashSet<Package> packages) {
			var result = new HashSet<Package>();
			packages = result;

			foreach (var amount in orderType.amounts) {
				if (!relevantPackagesPerPrefab.ContainsKey(amount.package)) return false;
				if (relevantPackagesPerPrefab[amount.package].Count < amount.amount) return false;
				result.AddAll(relevantPackagesPerPrefab[amount.package].Take(amount.amount));
			}
			return true;
		}

		public static int CountInstancesInStorage(Package packagePrefab) => relevantPackagesPerPrefab.ContainsKey(packagePrefab) ? relevantPackagesPerPrefab[packagePrefab].Count : 0;
	}
}