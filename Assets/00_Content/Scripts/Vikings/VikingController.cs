using System;
using System.Collections.Generic;
using Audio;
using Rounds;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Vikings {
	public class VikingController : SingletonBehaviour<VikingController> {
		[SerializeField] private Viking vikingPrefab;
		[SerializeField] private int maxVikings = 10;
		[SerializeField] private Transform exitPoint;
		[SerializeField] private QueueController queueController;
		[SerializeField] private GameObject[] vikingModelPrefabs;
		[SerializeField] private VikingData vikingData;

		private List<Viking> vikings = new List<Viking>();
		private float spawnDelay = 1f;
		private float spawnVariance;
		private float spawnTimer;

		public bool CanSpawn { get; set; } = true;
		public VikingScaling StatScaling { get; set; } = new VikingScaling();
		public Transform ExitPoint => exitPoint;
		public int VikingCount => vikings.Count;

		public event Action<Viking> VikingSpawned;

		private void Update() {
			if (!CanSpawn) return;

			spawnTimer -= Time.deltaTime;

			if (spawnTimer <= 0) {
				SpawnViking();
				spawnTimer = spawnDelay + Random.Range(-spawnVariance, spawnVariance);
			}
		}

		private void SpawnViking() {
			if (vikings.Count >= maxVikings) return;
			if (queueController != null && queueController.QueueSize >= queueController.QueueCapacity) return;

			Viking viking = Instantiate(vikingPrefab, transform);
			viking.ChangeModel(Util.RandomElement(vikingModelPrefabs));
			viking.SetScaling(StatScaling);
			viking.Data = vikingData;
			viking.LeaveTavern += OnLeaveTavern;

			vikings.Add(viking);

			if (queueController != null)
				queueController.AddToQueue(viking);

			VikingSpawned?.Invoke(viking);
			//AudioManager.PlayEffectSafe(SoundEffect.Gameplay_GuestEnter);
		}

		private void OnLeaveTavern(Viking sender) {
			bool removed = vikings.Remove(sender);
			Debug.Assert(removed, "Known viking left");

			Destroy(sender.gameObject);
		}

		public void SetSpawnSettings(float delay, float variance) {
			spawnDelay = delay;
			spawnVariance = variance;
		}

		public void LeaveAllVikings() {
			foreach (Viking viking in vikings) {
				viking.Leave();
			}
		}
	}
}
