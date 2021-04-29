using System;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Rounds {
	public class ScoreCard : MonoBehaviour {
		[SerializeField] private TMP_Text roundNumberText;
		[SerializeField] private PlayerReadyCard readyCardPrefab;
		[SerializeField] private Transform readyCardRoot;

		private int playerCount;
		private PlayerReadyCard[] readyCards = new PlayerReadyCard[PlayerManager.MaxPlayers];
		private (PlayerInputHandler player, UnityAction handler)[] playerHandlers =
			new (PlayerInputHandler, UnityAction)[PlayerManager.MaxPlayers];

		private int roundNumber;

		public event Action OnNextRound;

		public int RoundNumber {
			get => roundNumber;
			set { roundNumber = Mathf.RoundToInt(roundNumber); }
		}

		private void Awake() {
			for (int i = 0; i < readyCards.Length; i++) {
				readyCards[i] = Instantiate(readyCardPrefab, readyCardRoot);
				readyCards[i].Name = "Player " + i;
				readyCards[i].gameObject.SetActive(false);
			}
		}

		private void OnDisable() {
			for (int i = 0; i < playerCount; i++) {
				playerHandlers[i].player.OnStart.RemoveListener(playerHandlers[i].handler);
				playerHandlers[i] = default;
			}
		}

		public void UpdateScoreCard(int round) {
			roundNumber = round;
			roundNumberText.text = RoundNumber.ToString();
		}

		public void Show() {
			playerCount = PlayerManager.Instance.Players.Count;
			for (int i = 0; i < readyCards.Length; i++) {
				readyCards[i].gameObject.SetActive(i < playerCount);
				readyCards[i].Ready = false;
			}

			for (int i = 0; i < PlayerManager.Instance.Players.Count; i++) {
				PlayerInputHandler player = PlayerManager.Instance.Players[i].GetComponent<PlayerInputHandler>();

				int playerIndex = i;
				void OnStart() => HandleOnPlayerStart(playerIndex);
				player.OnStart.AddListener(OnStart);
				playerHandlers[i] = (player, OnStart);
			}

			gameObject.SetActive(true);
		}

		private void HandleOnPlayerStart(int playerIndex) {
			readyCards[playerIndex].ToggleReady();

			if (readyCards.Take(playerCount).All(card => card.Ready))
				GoToNextRound();
		}

		private void GoToNextRound() {
			OnNextRound?.Invoke();
			gameObject.SetActive(false);
		}
	}
}