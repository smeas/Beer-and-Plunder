using System;
using Player;
using Taverns;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Vikings;

namespace Rounds {
	public class RoundController : SingletonBehaviour<RoundController> {
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private Tavern tavern;
		[SerializeField] private VikingController vikingController;
		[SerializeField] private ScoreCard scoreCardPrefab;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;

		private ScoreCard scoreCard;
		private float roundTimer;
		private int currentRound = 1;

		public event Action OnRoundOver;

		private void Start() {
			scoreCard = Instantiate(scoreCardPrefab);
			scoreCard.gameObject.SetActive(false);

			tavern.OnBankrupcy += HandleOnTavernBankrupt;
			tavern.OnDestroyed += HandleOnTavernDestroyed;
			scoreCard.OnNextRound += HandleOnNextRound;

			roundTimer = roundDuration;

			SendNextDifficulty();
		}

		private void Update() {
			roundTimer = Mathf.Max(0, roundTimer - Time.deltaTime);

			if (roundTimer <= 0) RoundWon();
		}

		private void RoundWon() {
			OnRoundOver?.Invoke();

			// TODO: Wait for all vikings to leave before continuing.
			ShowScoreCard();
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

		private void ShowScoreCard() {
			scoreCard.UpdateScoreCard(currentRound);
			scoreCard.gameObject.SetActive(true);

			vikingController.CanSpawn = false;

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("UI");
			}
		}

		private void HandleOnNextRound() {
			vikingController.CanSpawn = true;

			foreach ( PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("Game");
			}

			SetUpNextRound();
		}

		private void SetUpNextRound() {
		currentRound++;
		SendNextDifficulty();
		roundTimer = roundDuration;
		}

		private void HandleOnTavernDestroyed() {
		throw new System.NotImplementedException();
		}

		private void HandleOnTavernBankrupt() {
		throw new System.NotImplementedException();
		}
	}
}