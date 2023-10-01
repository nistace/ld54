using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LD54.Data {
	[CreateAssetMenu]
	public class OrderType : ScriptableObject {
		[SerializeField] protected PackageAmount[] _amounts;
		[SerializeField] protected float _expirationTime = 20;
		[SerializeField] protected int _points = 10;
		[SerializeField] protected int _credits = 10;

		public int points => _points;
		public int credits => _credits;
		public IEnumerable<PackageAmount> amounts => _amounts;
		public float expirationTime => _expirationTime;

		public bool TryGetPrefabIfRequired(Package packageInstance, out Package prefab) => prefab = amounts.Select(t => t.package).FirstOrDefault(t => t.HasSameTypeAs(packageInstance));
		public int GetRequiredAmount(Package package) => _amounts.FirstOrDefault(t => t.package == package)?.amount ?? 0;

		[Serializable] public class PackageAmount {
			[SerializeField] protected Package _package;
			[SerializeField] protected int _amount = 1;

			public Package package => _package;
			public int amount => _amount;
		}
	}
}