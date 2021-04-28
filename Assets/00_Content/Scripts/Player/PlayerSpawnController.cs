using System.Linq;
using UnityEngine;

namespace Player {
	public class PlayerSpawnController : MonoBehaviour {
		private Transform[] spawnPoints;

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

		private void OnDestroy() {
			if (PlayerManager.Instance != null)
				PlayerManager.Instance.PlayerJoined -= HandlePlayerJoined;
		}

		private void HandlePlayerJoined(PlayerComponent player) {
			player.SpawnPoint = spawnPoints[Mathf.Min(player.PlayerId, spawnPoints.Length)];
			player.Respawn();
		}
	}
}