using System.Linq;
using Interactables;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace World {
	public class GoblinController : SingletonBehaviour<GoblinController> {
		[SerializeField] private Goblin goblinPrefab;
		[SerializeField] private int targetCount;

		private Transform[] spawnPoints;

		protected override void Awake() {
			base.Awake();

			spawnPoints = transform.Cast<Transform>().ToArray();
			if (spawnPoints.Length < 2)
				Debug.LogError("Not enough goblin spawn points", this);
		}

		private void Update() {
			if (Keyboard.current.gKey.wasPressedThisFrame)
				SpawnGoblin();
		}

		private void SpawnGoblin() {
			if (spawnPoints.Length == 0 || Coin.AllCoins.Count == 0) return;

			Transform spawnPoint = Util.RandomElement(spawnPoints);
			Goblin goblin = Instantiate(goblinPrefab, spawnPoint.position, spawnPoint.rotation);

			// Pick a different exit
			Transform exitPoint = spawnPoint;
			if (spawnPoints.Length >= 2) {
				do exitPoint = Util.RandomElement(spawnPoints);
				while (exitPoint == spawnPoint);
			}

			goblin.PickRandomTargetsAndGo(targetCount, exitPoint.position);
		}
	}
}