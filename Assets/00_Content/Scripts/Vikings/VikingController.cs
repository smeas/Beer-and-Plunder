using System;
using Rounds;
using UnityEngine;

namespace Vikings {
	public class VikingController : MonoBehaviour {
		[SerializeField] private Viking vikingPrefab;
		[SerializeField] private Transform[] spawnPoints;

		private Viking[] spawnedVikings;
		private float spawnTimer;

		public bool CanSpawn { get; set; } = true;
		public float SpawnDelay { get; set; } = 1f;
		public VikingScaling StatScaling { get; set; } = new VikingScaling();

		private void Start() {
			spawnedVikings = new Viking[spawnPoints.Length];
		}

		private void Update() {
			if (!CanSpawn) return;

			spawnTimer = Mathf.Max(0, spawnTimer - Time.deltaTime);

			if (spawnTimer <= 0) {
				SpawnViking();
				spawnTimer = SpawnDelay;
			}
		}

		private void SpawnViking() {
			int index = GetSpawnIndex();

			if (index == -1) return;

			Vector3 position = spawnPoints[index].position;
			Viking viking = Instantiate(vikingPrefab, position, Quaternion.identity, transform);
			viking.StatScaling = StatScaling;
			spawnedVikings[index] = viking;

			viking.LeaveTavern += OnLeaveTavern;
		}

		private int GetSpawnIndex() {
			for (int i = 0; i < spawnedVikings.Length; i++) {
				if (spawnedVikings[i] == null)
					return i;
			}

			return -1;
		}

		private void OnLeaveTavern(Viking sender) {
			int index = Array.IndexOf(spawnedVikings, sender);
			if (index == -1) return;

			spawnedVikings[index] = null;
			Destroy(sender.gameObject);
		}
	}
}
