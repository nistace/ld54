using System;
using UnityEngine;

namespace LD54.Data {
	[Serializable]
	public class PackageShape {
		[SerializeField] protected Line[] lines;

		public bool this[int x, int y] => y >= 0 && y < lines.Length && lines[y][x];

		public int width => lines.Length > 0 ? lines[0].size : 0;
		public int length => lines.Length;

		[Serializable] protected class Line {
			[SerializeField] protected bool[] cells;

			public bool this[int x] => x >= 0 && x < cells.Length && cells[x];
			public int size => cells.Length;
		}
	}
}