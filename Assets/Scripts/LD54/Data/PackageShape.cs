using System;
using System.Collections.Generic;
using NiUtils.Extensions;
using UnityEngine;

namespace LD54.Data {
	[Serializable]
	public class PackageShape {
		[SerializeField] protected Line[] lines;

		public bool this[int x, int y] {
			get => y >= 0 && y < lines.Length && lines[y][x];
			private set => lines[y][x] = value;
		}

		private Dictionary<int, PackageShape> shapeRotations { get; } = new Dictionary<int, PackageShape>();
		public int width => lines.Length > 0 ? lines[0].size : 0;
		public int length => lines.Length;

		private PackageShape CreateClockwiseRotation() {
			var rotatedShape = new PackageShape { lines = width.CreateArray(_ => new Line(length)) };
			for (var oldX = 0; oldX < width; ++oldX)
			for (var oldY = 0; oldY < length; ++oldY) {
				rotatedShape[oldY, width - oldX - 1] = this[oldX, oldY];
			}

			return rotatedShape;
		}

		public void ClearRotations() => shapeRotations.Clear();

		public PackageShape GetRotated(int timesClockwise) {
			var positiveTimesClockwise = timesClockwise;
			while (positiveTimesClockwise < 0) positiveTimesClockwise += 4;
			positiveTimesClockwise %= 4;
			if (shapeRotations.Count == 0) {
				shapeRotations.Add(0, this);
				for (var i = 1; i < 4; ++i)
					shapeRotations.Add(i, shapeRotations[i - 1].CreateClockwiseRotation());
				for (var i = 1; i < 4; ++i)
				for (var j = 1; j < 4; ++j)
					shapeRotations[i].shapeRotations.Add(j, shapeRotations[(j + i) % 4]);
			}
			return shapeRotations[positiveTimesClockwise];
		}

		[Serializable] protected class Line {
			[SerializeField] protected bool[] cells;

			public Line() : this(0) { }
			public Line(int cellCount) => cells = new bool[cellCount];

			public bool this[int x] {
				get => x >= 0 && x < cells.Length && cells[x];
				set => cells[x] = value;
			}

			public int size => cells.Length;
		}
	}
}