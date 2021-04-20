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

			vikingController.CanSpawn = false;
			int i = 0;

			PlayerManager.Instance.Players[i].GetComponent
		}
		//maybe I should put a comment here, seems like Jon felt that, to explain, so I can explain how this is accessed?
		private void HandleOnNextRound() {
			vikingController.CanSpawn = true;

			//f� tag spelaren, f� tag p� inputmanager, ge en str�ng som heter UI.
			//h�mta fr�n PlayerManager, alla spelare, sen, var f�r sig st�ng av/s�t p� deras input, byt den, med str�ngar.

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