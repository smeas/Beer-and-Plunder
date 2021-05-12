using System;
using Player;
using Rounds;
using UnityEngine;
using UnityEngine.Events;

namespace Menu {
	public class ReadySystem : MonoBehaviour {
		[SerializeField] private PlayerReadyCard readyCardPrefab;
		[SerializeField] private Transform readyCardRoot;

		[SerializeField]
		private PlayerReadyCard[] readyCards = new PlayerReadyCard[PlayerManager.MaxPlayers];

		private (PlayerInputHandler player, UnityAction handler)[] playerHandlers =
			new (PlayerInputHandler player, UnityAction handler)[PlayerManager.MaxPlayers];

		public event Action AllReady;

		private void Awake() {
			for (int i = 0; i < readyCards.Length; i++) {
				readyCards[i] = Instantiate(readyCardPrefab, readyCardRoot);
				readyCards[i].Name = "Player " + (i + 1);
				readyCards[i].gameObject.SetActive(false);
			}
		}

		private void OnDisable() {
			for (int i = 0; i < playerHandlers.Length; i++) {
				if (playerHandlers[i] != default) {
					playerHandlers[i].player.OnStart.RemoveListener(playerHandlers[i].handler);
					playerHandlers[i] = default;
				}
			}
		}

		public void Initialize() {
			int playerCount = PlayerManager.Instance.Players.Count;
			for (int i = 0; i < readyCards.Length; i++) {
				readyCards[i].gameObject.SetActive(i < playerCount);
				readyCards[i].Ready = false;
			}

			for (int i = 0; i < PlayerManager.Instance.Players.Count; i++) {
				PlayerInputHandler player =
					PlayerManager.Instance.Players[i].GetComponent<PlayerInputHandler>();

				int playerIndex = i;
				void OnStart() => HandleOnPlayerStart(playerIndex);
				player.OnStart.AddListener(OnStart);
				playerHandlers[i] = (player, OnStart);
			}
		}

		private void HandleOnPlayerStart(int playerIndex) {
			readyCards[playerIndex].ToggleReady();

			foreach (PlayerReadyCard readyCard in readyCards) {
				if (!readyCard.isActiveAndEnabled) continue;
				if (!readyCard.Ready) return;
			}

			AllReady?.Invoke();
		}
	}
}
