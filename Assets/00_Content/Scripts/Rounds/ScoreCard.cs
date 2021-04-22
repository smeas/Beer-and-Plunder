using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rounds {
	public class ScoreCard : MonoBehaviour {
		[SerializeField] private TMP_Text roundNumberText;
		[SerializeField] private Button startButton;

		private int roundNumber;

		public event Action OnNextRound;

		public int RoundNumber {
			get => roundNumber;
			set { roundNumber = Mathf.RoundToInt(roundNumber); }
		}

		private void OnEnable() {
			if (EventSystem.current != null)
				EventSystem.current.SetSelectedGameObject(startButton.gameObject);
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