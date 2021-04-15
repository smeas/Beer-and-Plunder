using Player;
using ScriptableObjects;
using UnityEngine;
using Taverns;
using Vikings;

namespace Rounds {
	public class RoundController : MonoBehaviour {
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private Tavern tavern;
		[SerializeField] private VikingController vikingController;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;

		private float roundTimer;
		private int currentRound = 1;

		private void Start() {
			tavern.onBankrupcy += HandleOnTavernBankrupt;
			tavern.onDestroyed += HandleOnTavernDestroyed;

			roundTimer = roundDuration;
			SendNextDifficulty();
		}

		private void Update() {
			roundTimer = Mathf.Max(0, roundTimer - Time.deltaTime);

			if (roundTimer <= 0) {
				currentRound++;
				SendNextDifficulty();
				roundTimer = roundDuration;
			}
		}

		private void SendNextDifficulty() {
			if (vikingController == null) {
				Debug.Assert(false, "Roundcontroller have access to a vikingController");
				return;
			}

			ScalingData difficulty = playerDifficulties[PlayerManager.Instance && PlayerManager.Instance.NumPlayers > 0
				? PlayerManager.Instance.NumPlayers - 1
				: 0];

			vikingController.SpawnDelay = difficulty.ScaledSpawnDelay(currentRound);
			vikingController.StatScaling = new VikingScaling(difficulty, currentRound);
		}

		private void HandleOnTavernDestroyed() {
			throw new System.NotImplementedException();
		}

		private void HandleOnTavernBankrupt() {
			throw new System.NotImplementedException();
		}
	}
}
