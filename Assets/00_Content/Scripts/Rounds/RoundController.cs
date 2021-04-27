using System;
using Interactables;
using Player;
using Taverns;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Vikings;

namespace Rounds {
	public class RoundController : SingletonBehaviour<RoundController> {
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private ScoreCard scoreCardPrefab;
		[SerializeField] private GameObject HUDPrefab;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;
		[SerializeField] private int requiredMoney = 250;

		private ScoreCard scoreCard;
		private float roundTimer;
		private int currentRound = 1;
		private bool isRoundActive = true;

		public ScalingData CurrentDifficulty =>
			playerDifficulties[PlayerManager.Instance && PlayerManager.Instance.NumPlayers > 0
			? PlayerManager.Instance.NumPlayers - 1
			: 0];

		public event Action OnRoundOver;

		public float RoundTimer => roundTimer;
		public int RequiredMoney => requiredMoney;

		private void Start() {
			scoreCard = Instantiate(scoreCardPrefab);
			scoreCard.gameObject.SetActive(false);

			Tavern.Instance.OnDestroyed += HandleOnTavernDestroyed;
			scoreCard.OnNextRound += HandleOnNextRound;

			roundTimer = roundDuration;

			SendNextDifficulty();
		}

		private void Update() {
			if (!isRoundActive) return;

			roundTimer = Mathf.Max(0, roundTimer - Time.deltaTime);

			if (roundTimer <= 0) RoundWon();
		}

		private void RoundWon() {
			isRoundActive = false;
			OnRoundOver?.Invoke();

			// TODO: Wait for all vikings to leave before continuing.
			ShowScoreCard();
		}

		private void SendNextDifficulty() {
			if (VikingController.Instance == null) {
				Debug.Assert(false, "Roundcontroller have access to a vikingController");
				return;
			}

			ScalingData difficulty = CurrentDifficulty;

			VikingController.Instance.SetSpawnSettings(difficulty.ScaledSpawnDelay(currentRound), difficulty.spawnDelayVariance);
			VikingController.Instance.StatScaling = new VikingScaling(difficulty, currentRound);
		}

		private void ShowScoreCard() {
			if (Tavern.Instance != null && Tavern.Instance.Money < requiredMoney) {
				Debug.Log("Required money goal was not reached.");
				// TODO: Maybe show the score card first?
				GameOver();
				//return;
			}

			scoreCard.UpdateScoreCard(currentRound);
			scoreCard.gameObject.SetActive(true);

			VikingController.Instance.CanSpawn = false;

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("UI");
			}
		}

		private void HandleOnNextRound() {
			VikingController.Instance.CanSpawn = true;

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("Game");
			}

			isRoundActive = true;
			currentRound++;
			SendNextDifficulty();
			roundTimer = roundDuration;

			if (Tavern.Instance != null)
				Tavern.Instance.Money = Tavern.Instance.StartingMoney;

			foreach (Table table in Table.AllTables) {
				table.Repair();
			}
		}

		private void GameOver() {
			// TODO
		}

		private void HandleOnTavernDestroyed() {
			throw new System.NotImplementedException();
		}
	}
}
