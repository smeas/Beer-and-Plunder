using System;
using DG.Tweening;
using Menu;
using TMPro;
using UnityEngine;

namespace Rounds {
	public class ScoreCard : MonoBehaviour {
		[SerializeField] private TMP_Text roundNumberText;
		[SerializeField] private ReadySystem readySystem;

		[SerializeField] private GameObject banner;
		[SerializeField] private float bannerEntryDuration = 1f;

		private int roundNumber;

		public event Action OnNextRound;

		public int RoundNumber {
			get => roundNumber;
			set { roundNumber = Mathf.RoundToInt(roundNumber); }
		}

		private void OnDisable() {
			readySystem.AllReady -= GoToNextRound;
		}

		public void UpdateScoreCard(int round) {
			roundNumber = round;
			roundNumberText.text = RoundNumber.ToString();
		}

		public void Show() {
			readySystem.Initialize();
			readySystem.AllReady += GoToNextRound;

			gameObject.SetActive(true);
			banner.transform.localScale = new Vector3(0, 0, 1);
			banner.transform.DOScale(Vector3.one, bannerEntryDuration).SetEase(Ease.OutBounce);
		}

		private void GoToNextRound() {
			OnNextRound?.Invoke();
			gameObject.SetActive(false);
		}
	}
}
