using System;
using UnityEngine;

namespace Vikings {
	public class VikingController : MonoBehaviour {
		[SerializeField] private Viking vikingPrefab;
		[SerializeField] private Transform[] spawnPoints;
		[SerializeField] private float spawnDelay = 2.5f;

		private Viking[] spawnedVikings;
		private float spawnTimer;

		public bool CanSpawn { get; set; } = true;

		private void Start() {
			spawnedVikings = new Viking[spawnPoints.Length];
		}

		private void Update() {
			if (!CanSpawn) return;

			spawnTimer = Mathf.Max(0, spawnTimer - Time.deltaTime);

			if (spawnTimer <= 0) {
				SpawnViking();
				spawnTimer = spawnDelay;
			}
		}

		private void SpawnViking() {
			int index = GetSpawnIndex();

			if (index == -1) return;

			Vector3 position = spawnPoints[index].position;
			Viking viking = Instantiate(vikingPrefab, position, Quaternion.identity, transform);

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
