using System;
using System.Collections.Generic;
using NiUtils.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LD54.Data {
	public class WaterPlane : MonoBehaviour {
		public static WaterPlane current { get; set; }

		[SerializeField] protected MeshFilter _meshFilter;
		[Range(1, 100), SerializeField] protected int _quality = 100;
		[SerializeField] protected float _size = 12;
		[SerializeField] protected float noiseCoefficient = 2;
		[SerializeField] protected float noiseOffset = -1;
		[SerializeField] protected float noiseSpeed = .1f;
		[SerializeField] protected float noiseSmooth = .001f;

		private Vector2 randomOffset { get; set; }
		private Vector3 centerOffset { get; set; }
		private int verticesPerRow => _quality + 1;

		private Queue<Transform> promptedClosestPositions { get; } = new Queue<Transform>();
		private Dictionary<Transform, Action<Vector3>> promptCallbacks { get; } = new Dictionary<Transform, Action<Vector3>>();

		private void Awake() {
			current = this;
		}

		private void Start() {
			Build();
		}

		[ContextMenu("Build")]
		private void Build() {
			var mesh = new Mesh();

			var vertices = new List<Vector3>();
			var triangles = new List<int>();

			centerOffset = new Vector3(-_size * .5f, 0, -_size * .5f);
			randomOffset = new Vector2(Random.value, Random.value);

			for (var x = 0; x <= _quality; x++)
			for (var y = 0; y <= _quality; y++) {
				vertices.Add(centerOffset + GetVertexLocalPosition(x, y));
				if (x > 0 && y > 0) {
					triangles.Add((y - 1) * verticesPerRow + x - 1);
					triangles.Add((y - 1) * verticesPerRow + x);
					triangles.Add(y * verticesPerRow + x - 1);
					triangles.Add(y * verticesPerRow + x - 1);
					triangles.Add((y - 1) * verticesPerRow + x);
					triangles.Add(y * verticesPerRow + x);
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			_meshFilter.mesh = mesh;
		}

		private void Update() {
			var vertices = _meshFilter.mesh.vertices;
			for (var index = 0; index < vertices.Length; index++) {
				vertices[index] = centerOffset + GetVertexLocalPosition(index / verticesPerRow, index % verticesPerRow);
			}
			_meshFilter.mesh.vertices = vertices;
			if (promptedClosestPositions.Count > 0) {
				var nextPromptTarget = promptedClosestPositions.Dequeue();
				var closestPosition = GetClosestVertexPosition(nextPromptTarget.position);
				var callback = promptCallbacks[nextPromptTarget];
				promptCallbacks.Remove(nextPromptTarget);
				callback.Invoke(closestPosition);
			}
		}

		private Vector3 GetVertexLocalPosition(float vertexX, float vertexZ) {
			var noiseX = noiseSpeed * Time.time + randomOffset.x + vertexX * noiseSmooth / _quality;
			var noiseY = noiseSpeed * Time.time + randomOffset.y + vertexZ * noiseSmooth / _quality;
			var y = noiseOffset + noiseCoefficient * Mathf.PerlinNoise(noiseX, noiseY);
			return new Vector3(vertexX * _size / _quality, y, vertexZ * _size / _quality);
		}

		private Vector3 GetClosestVertexPosition(Vector3 worldPosition) => _meshFilter.mesh.vertices.GetWithClosestScore(t => (worldPosition - t).sqrMagnitude, 0);

		public void GetClosestVertexPosition(Transform target, Action<Vector3> callback) {
			if (promptedClosestPositions.Contains(target)) return;
			if (callback == null) return;
			promptedClosestPositions.Enqueue(target);
			promptCallbacks.Add(target, callback);
		}
	}
}