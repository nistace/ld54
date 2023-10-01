using System.Collections.Generic;
using NiUtils.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LD54.Data {
	public class PackageReserve : MonoBehaviour, IInteractable {
		[SerializeField] protected Package _packagePrefab;
		[SerializeField] protected Transform _poolParent;
		[SerializeField] protected Vector3 _spawnOffset = Vector3.up;

		private List<Package> pool { get; } = new List<Package>();
		private HashSet<Package> activePackages { get; } = new HashSet<Package>();

		private void Start() {
			_poolParent.gameObject.SetActive(false);
		}

		public Package Spawn() {
			if (pool.Count == 0) pool.Add(Instantiate(_packagePrefab, _spawnOffset, Quaternion.identity, _poolParent));

			var spawned = pool[0];
			pool.RemoveAt(0);

			spawned.Init();
			spawned.transform.SetParent(null);
			spawned.transform.position = transform.position + _spawnOffset;
			spawned.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
			SetListenersActive(spawned, true);
			activePackages.Add(spawned);
			return spawned;
		}

		private void SetListenersActive(Package package, bool active) {
			package.onDelivered.SetListenerActive(Pool, active);
		}

		private void Pool(Package package) {
			if (!activePackages.Contains(package)) return;
			SetListenersActive(package, false);
			activePackages.Remove(package);
			pool.Add(package);
			package.transform.SetParent(_poolParent);
		}
	}
}