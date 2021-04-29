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
		//right, so, I will need to have a reference here to the canvas/panel that I should use for game over.
		// it has a certain advantage since I already have "stop & pause" somewhere here, and could possibly make all of that into one function to use more than once.
		//So, this has a game over state, but it can, infact, only let you restart OR send you to main menu, which is a scene handled by the SceneLoadManager correct?
		//so Create up the MainMenu from the prefab. Have buttons on the prefab. Have functions on the button that taks you to other states.
		//So you will need to investigate what will remain if you go between scenes like this. Will singletons or such remain? What will need to be set-up again?
		//In some way it is perhaps strange to have a menuController and not let that handle this no?
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private ScoreCard scoreCardPrefab;
		[SerializeField] private GameObject HUDPrefab;
		[SerializeField] private GameObject gameOverPanelPrefab;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;

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

		private void Start() {
			scoreCard = Instantiate(scoreCardPrefab);
			scoreCard.gameObject.SetActive(false);
			gameOverPanelPrefab = Instantiate(gameOverPanelPrefab);
			gameOverPanelPrefab.gameObject.SetActive(false);

			Tavern.Instance.OnBankrupcy += HandleOnTavernBankrupt;
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
			DisableGamePlay();
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

		private void DisableGamePlay() {
			VikingController.Instance.CanSpawn = false;

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("UI");
			}
		}

		private void EnableGamePlay() {
			VikingController.Instance.CanSpawn = true;

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("Game");
			}
		}
		private void ShowScoreCard() {
			scoreCard.UpdateScoreCard(currentRound);
			scoreCard.gameObject.SetActive(true);
		}

		private void HandleOnNextRound() {
			EnableGamePlay();
			isRoundActive = true;
			currentRound++;
			SendNextDifficulty();
			roundTimer = roundDuration;

			foreach (Table table in Table.AllTables) {
				table.Repair();
			}
		}

		private void HandleOnTavernDestroyed() {
			isRoundActive = false;
			DisableGamePlay();

			Debug.Log("RoundController notices that the tavern was destroyed.");
			gameOverPanelPrefab.gameObject.SetActive(true);
		}

		private void HandleOnTavernBankrupt() {
			isRoundActive = false;
			DisableGamePlay();

			Debug.Log("RoundController notices that the tavern went bankrupt.");
			gameOverPanelPrefab.gameObject.SetActive(true);
		}
	}
}
