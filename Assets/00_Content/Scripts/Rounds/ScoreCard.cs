using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rounds {
	public class ScoreCard : MonoBehaviour {
		[SerializeField] private TMP_Text roundNumberText;

		private int roundNumber;

		public event Action OnNextRound;

		public int RoundNumber {
			get => roundNumber;
			set { roundNumber = Mathf.RoundToInt(roundNumber); }
		}

		private void Update() {

			//TODO: Get number of active players,
			//TODO: Enable that many icons on UI
			//TODO: Wait for input from each player
			//TODO: Have their icon switch over when they give correct input
			//TODO: Have the icon switch back if player gives same input again.
			//TODO: When all have given input start next round.
		}

		public void UpdateScoreCard(int round) {
			roundNumber = round;
			roundNumberText.text = RoundNumber.ToString();
		}

		public void GoToNextRound() {
			OnNextRound?.Invoke();
			
			gameObject.SetActive(false);
		}
	}
}