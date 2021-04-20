using System;
using Player;
using Taverns;
using UnityEngine;
using Utilities;
using Vikings;

namespace Rounds {
	public class RoundController : SingletonBehaviour<RoundController> {
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private Tavern tavern;
		[SerializeField] private VikingController vikingController;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;

		private float roundTimer;
		private int currentRound = 1;

		public event Action OnRoundOver;

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
				OnRoundOver?.Invoke();

				// TODO: Wait for all vikings to leave before starting next round.
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

			vikingController.SetSpawnSettings(difficulty.ScaledSpawnDelay(currentRound), difficulty.spawnDelayVariance);
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
