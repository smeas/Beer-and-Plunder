using System;
using Player;
using Rounds;
using UnityEngine;
using UnityEngine.Events;

namespace Menu {
	public class ReadySystem : MonoBehaviour {
		[SerializeField] private PlayerReadyCard[] readyCards = new PlayerReadyCard[PlayerManager.MaxPlayers];

		private (PlayerInputHandler player, UnityAction handler)[] playerHandlers =
			new (PlayerInputHandler player, UnityAction handler)[PlayerManager.MaxPlayers];

		public event Action AllReady;

		private void OnEnable() {
			PlayerManager.Instance.PlayerJoined += OnPlayerJoined;
			PlayerManager.Instance.PlayerLeft += OnPlayerLeft;
		}

		private void OnDisable() {
			for (int i = 0; i < playerHandlers.Length; i++) {
				if (playerHandlers[i] != default) {
					playerHandlers[i].player.OnStart.RemoveListener(playerHandlers[i].handler);
					playerHandlers[i] = default;
				}
			}

			if (PlayerManager.Instance != null) {
				PlayerManager.Instance.PlayerJoined -= OnPlayerJoined;
				PlayerManager.Instance.PlayerLeft -= OnPlayerLeft;
			}
		}

		public void Initialize() {
			for (int i = 0; i < readyCards.Length; i++)
				readyCards[i].Ready = false;

			for (int i = 0; i < PlayerManager.Instance.Players.Count; i++)
				SetupPlayer(PlayerManager.Instance.Players[i], i);
		}

		private void OnPlayerJoined(PlayerComponent player) {
			for (int i = 0; i < playerHandlers.Length; i++) {
				if (playerHandlers[i] == default) {
					SetupPlayer(player, i);
					break;
				}
			}
		}

		private void SetupPlayer(PlayerComponent player, int index) {
			PlayerInputHandler playerInputHandler = player.GetComponent<PlayerInputHandler>();
			void OnStart() => HandleOnPlayerStart(index);
			playerInputHandler.OnStart.AddListener(OnStart);
			playerHandlers[index] = (playerInputHandler, OnStart);

			readyCards[index].TrackPlayer(player);
			readyCards[index].gameObject.SetActive(true);
		}

		private void OnPlayerLeft(PlayerComponent plr) {
			PlayerInputHandler player = plr.GetComponent<PlayerInputHandler>();

			for (int i = 0; i < playerHandlers.Length; i++) {
				if (playerHandlers[i].player != player) continue;

				playerHandlers[i].player.OnStart.RemoveListener(playerHandlers[i].handler);
				playerHandlers[i] = default;
				readyCards[i].gameObject.SetActive(false);
				break;
			}

			foreach (PlayerReadyCard card in readyCards)
				card.Ready = false;
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
