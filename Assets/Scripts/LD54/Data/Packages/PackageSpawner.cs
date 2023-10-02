using System.Collections.Generic;
using System.Linq;
using LD54.Data;
using NiUtils.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class PackageSpawner : MonoBehaviour {
	public static PackageSpawner current { get; private set; }

	[SerializeField] protected Vector3 spawnAreaCenter;
	[SerializeField] protected Vector3 spawnAreaSize;
	[SerializeField] protected Vector3 spawnForceVector;
	[SerializeField] protected float spawnMinForce;
	[SerializeField] protected float spawnMaxForce;
	[SerializeField] protected Transform _poolParent;

	private Dictionary<Package, List<Package>> pool { get; } = new Dictionary<Package, List<Package>>();
	private Dictionary<Package, Package> spawnedAndActive { get; } = new Dictionary<Package, Package>();

	private void Awake() {
		current = this;
	}

	public void Init() {
		_poolParent.gameObject.SetActive(false);
	}

	private void Pool(Package package) {
		if (!spawnedAndActive.ContainsKey(package)) return;
		SetListenersActive(package, false);
		var prefab = spawnedAndActive[package];
		spawnedAndActive.Remove(package);
		pool[prefab].Add(package);
		package.transform.SetParent(_poolParent);
	}

	public void SpawnForOrder(OrderType order) {
		foreach (var packageAmount in order.amounts) {
			for (var i = 0; i < packageAmount.amount; ++i) {
				Spawn(packageAmount.package);
			}
		}
	}

	public Package GetAnyActivePackage() => spawnedAndActive.Keys.FirstOrDefault();

	private void Spawn(Package prefab) {
		if (!pool.ContainsKey(prefab)) pool.Add(prefab, new List<Package>());
		if (pool[prefab].Count == 0) pool[prefab].Add(Instantiate(prefab, _poolParent));

		var spawned = pool[prefab][0];
		pool[prefab].RemoveAt(0);
		spawnedAndActive.Add(spawned, prefab);

		spawned.Init();
		spawned.transform.SetParent(null);
		var spawnOffset = new Vector3(Random.Range(0, spawnAreaSize.x), Random.Range(0, spawnAreaSize.y), Random.Range(0, spawnAreaSize.z)) - spawnAreaSize * .5f;
		spawned.transform.position = spawnAreaCenter + spawnOffset;
		spawned.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
		spawned.rigidbody.velocity = spawnForceVector * Random.Range(spawnMinForce, spawnMaxForce);
		SetListenersActive(spawned, true);
	}

	private void SetListenersActive(Package package, bool active) {
		package.onDelivered.SetListenerActive(Pool, active);
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.color = new Color(.6f, 1, .7f, .8f);
		Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
		var minForce = spawnAreaCenter + spawnForceVector * spawnMinForce;
		var maxForce = spawnAreaCenter + spawnForceVector * spawnMaxForce;
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(spawnAreaCenter, minForce);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(minForce, maxForce);
	}
#endif
}