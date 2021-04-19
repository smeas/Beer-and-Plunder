using System;
using Rounds;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Vikings {
	public class VikingController : MonoBehaviour {
		[SerializeField] private Viking vikingPrefab;
		[SerializeField] private Transform[] spawnPoints;

		private Viking[] spawnedVikings;
		private float spawnDelay = 1f;
		private float spawnVariance;
		private float spawnTimer;

		public bool CanSpawn { get; set; } = true;
		public VikingScaling StatScaling { get; set; } = new VikingScaling();

		private void Start() {
			spawnedVikings = new Viking[spawnPoints.Length];
		}

		private void Update() {
			if (!CanSpawn) return;

			spawnTimer -= Time.deltaTime;

			if (spawnTimer <= 0) {
				SpawnViking();
				spawnTimer = spawnDelay + Random.Range(-spawnVariance, spawnVariance);
			}
		}

		private void SpawnViking() {
			int index = GetSpawnIndex();

			if (index == -1) return;

			Vector3 position = spawnPoints[index].position;
			Viking viking = Instantiate(vikingPrefab, position, Quaternion.identity, transform);
			viking.SetScaling(StatScaling);
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

		public void SetSpawnSettings(float delay, float variance) {
			spawnDelay = delay;
			spawnVariance = variance;
		}
	}
}
