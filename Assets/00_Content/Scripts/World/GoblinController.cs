using System.Collections.Generic;
using System.Linq;
using Interactables;
using Rounds;
using UnityEngine;
using Utilities;

namespace World {
	public class GoblinController : SingletonBehaviour<GoblinController> {
		[SerializeField] private Goblin goblinPrefab;
		[SerializeField] private int maxGoblins = 1;
		[SerializeField] private int goblinTargetCount = 6;
		[SerializeField] private int requiredCoinsToSpawn = 6;
		[SerializeField, MinMaxRange(0, 60)]
		private Vector2 spawnDelay = new Vector2(20, 30);
		[SerializeField] private float spawnRetryDelay = 5f;

		private Transform[] spawnPoints;
		private List<Goblin> goblins = new List<Goblin>();
		private float spawnTimer;

		protected override void Awake() {
			base.Awake();

			spawnPoints = transform.Cast<Transform>().ToArray();
			if (spawnPoints.Length < 2)
				Debug.LogError("Not enough goblin spawn points", this);

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += ResetTimer;

			ResetTimer();
		}

		private void Update() {
			if (goblins.Count >= maxGoblins)
				return;

			if (RoundController.Instance != null && !RoundController.Instance.IsRoundActive)
				return;

			spawnTimer -= Time.deltaTime;
			if (spawnTimer <= 0) {
				if (TrySpawnGoblin())
					ResetTimer();
				else
					spawnTimer = spawnRetryDelay;
			}
		}

		private void ResetTimer() {
			spawnTimer = Random.Range(spawnDelay.x, spawnDelay.y);
		}

		private bool TrySpawnGoblin() {
			if (spawnPoints.Length == 0 || Coin.AllCoins.Count < requiredCoinsToSpawn)
				return false;

			Transform spawnPoint = Util.RandomElement(spawnPoints);
			Goblin goblin = Instantiate(goblinPrefab, spawnPoint.position, spawnPoint.rotation);

			goblin.OnLeave += HandleOnGoblinLeave;
			goblins.Add(goblin);

			// Pick a different exit
			Transform exitPoint = spawnPoint;
			if (spawnPoints.Length >= 2) {
				do exitPoint = Util.RandomElement(spawnPoints);
				while (exitPoint == spawnPoint);
			}

			goblin.PickRandomTargetsAndGo(goblinTargetCount, exitPoint.position);
			return true;
		}

		private void HandleOnGoblinLeave(Goblin goblin) {
			goblins.Remove(goblin);
		}
	}
}