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