using System;
using Player;
using UnityEngine;
using Taverns;
using Vikings;

namespace Rounds {
	public class RoundController : MonoBehaviour {
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private Tavern tavern;
		[SerializeField] private VikingController vikingController;
		[SerializeField] private ScoreCard scoreCardPrefab;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;

		private ScoreCard scoreCard;
		private float roundTimer;
		private int currentRound = 1;

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

			vikingController.SpawnDelay = difficulty.ScaledSpawnDelay(currentRound);
			vikingController.StatScaling = new VikingScaling(difficulty, currentRound);
		}

		private void ShowScoreCard() {
			scoreCard.UpdateScoreCard(currentRound);
			scoreCard.gameObject.SetActive(true);

			//might need to turn of some sort of input or components here.
			//switch some sort of input to ui thing-y here...
			//also, disable the vikingController I think?

			vikingController.gameObject.SetActive(false);

		}

		private void HandleOnNextRound() {
			vikingController.gameObject.SetActive(true);

			SetUpNextRound();
			//turn on components input etc again.
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