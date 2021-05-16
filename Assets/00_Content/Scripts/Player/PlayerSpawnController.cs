using System.Linq;
using UnityEngine;
using Utilities;

namespace Player {
	public class PlayerSpawnController : SingletonBehaviour<PlayerSpawnController> {
		private Transform[] spawnPoints;

		public Transform[] SpawnPoints => spawnPoints;

		private void Start() {
			// Array of child transforms
			spawnPoints = transform.Cast<Transform>().ToArray();

			if (spawnPoints.Length < PlayerManager.MaxPlayers)
				Debug.LogError("Not enough spawn points for everyone!", this);

			if (PlayerManager.Instance == null)
				return;

			foreach (PlayerComponent player in PlayerManager.Instance.Players)
				HandlePlayerJoined(player);

			PlayerManager.Instance.PlayerJoined += HandlePlayerJoined;
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			if (PlayerManager.Instance != null)
				PlayerManager.Instance.PlayerJoined -= HandlePlayerJoined;
		}

		private void HandlePlayerJoined(PlayerComponent player) {
			player.SpawnPoint = spawnPoints[Mathf.Min(player.PlayerId, spawnPoints.Length)];
			player.Respawn();
		}
	}
}
