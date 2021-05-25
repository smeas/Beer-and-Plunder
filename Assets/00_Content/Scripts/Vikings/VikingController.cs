using System;
using System.Collections;
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

		private void Start() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnNewRoundStart += HandleOnRoundStart;

			ResetSpawnTimer();
			HandleOnRoundStart();
		}

		private void Update() {
			if (!CanSpawn) return;

			spawnTimer -= Time.deltaTime;

			if (spawnTimer <= 0) {
				SpawnViking();
				ResetSpawnTimer();
			}
		}

		private void HandleOnRoundStart() {
			if (RoundController.Instance == null)
				return;

			int vikingsToSpawn = Mathf.Min(maxVikings, RoundController.Instance.CurrentDifficulty.ScaledInitialVikings(RoundController.Instance.CurrentRound));
			StartCoroutine(CoSpawnVikings(vikingsToSpawn, 1f));
		}

		private void ResetSpawnTimer() {
			spawnTimer = spawnDelay + Random.Range(-spawnVariance, spawnVariance);
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

		private IEnumerator CoSpawnVikings(int count, float delay) {
			for (int i = 0; i < count; i++) {
				if (vikings.Count >= maxVikings)
					yield break;

				SpawnViking();
				ResetSpawnTimer();

				yield return new WaitForSeconds(delay);
			}
		}
	}
}
