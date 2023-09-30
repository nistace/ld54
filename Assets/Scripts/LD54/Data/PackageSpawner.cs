using System.Collections.Generic;
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

	private float nextSpawnProgress { get; set; }
	private HashSet<Package> spawnablePackages { get; } = new HashSet<Package>();
	private List<Package> nextSpawns { get; } = new List<Package>();
	private Dictionary<Package, List<Package>> pool { get; } = new Dictionary<Package, List<Package>>();
	private Dictionary<Package, Package> spawnedAndActive { get; } = new Dictionary<Package, Package>();

	private void Awake() {
		current = this;
	}

	public void Init() {
		_poolParent.gameObject.SetActive(false);
		spawnablePackages.Clear();
		spawnablePackages.AddAll(GameSessionData.current.config.spawnablePackages);
		Package.onDelivered.AddListenerOnce(HandlePackageDelivered);
	}

	private void HandlePackageDelivered(Package package) {
		if (!spawnedAndActive.ContainsKey(package)) return;
		var prefab = spawnedAndActive[package];
		spawnedAndActive.Remove(package);
		pool[prefab].Add(package);
		package.transform.SetParent(_poolParent);
	}

	public void Disable() { }

	[ContextMenu("Spawn Random")]
	public void SpawnRandom() {
		if (spawnablePackages.Count == 0) return;
		Debug.Log($"Spawn {Time.time:0.00}");
		if (nextSpawns.Count == 0) {
			nextSpawns.AddRange(spawnablePackages);
			nextSpawns.Sort(_ => Random.value);
		}

		var prefab = nextSpawns[0];
		nextSpawns.RemoveAt(0);

		if (!pool.ContainsKey(prefab)) pool.Add(prefab, new List<Package>());
		if (pool[prefab].Count == 0) pool[prefab].Add(Instantiate(prefab, _poolParent));

		var spawned = pool[prefab][0];
		pool[prefab].RemoveAt(0);
		spawnedAndActive.Add(spawned, prefab);

		spawned.transform.SetParent(null);
		var spawnOffset = new Vector3(Random.Range(0, spawnAreaSize.x), Random.Range(0, spawnAreaSize.y), Random.Range(0, spawnAreaSize.z)) - spawnAreaSize * .5f;
		spawned.transform.position = spawnAreaCenter + spawnOffset;
		spawned.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
		spawned.rigidbody.velocity = spawnForceVector * Random.Range(spawnMinForce, spawnMaxForce);
		spawned.transform.localScale = Vector3.one;
		spawned.locked = false;
	}

	private void Update() {
		if (!GameSessionData.isPlaying) return;
		nextSpawnProgress += GameSessionData.current.config.spawnPerSecondCurve.Evaluate(GameSessionData.gameTime) * Time.deltaTime;
		while (nextSpawnProgress > 1) {
			SpawnRandom();
			nextSpawnProgress--;
		}
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