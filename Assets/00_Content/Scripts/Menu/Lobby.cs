using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Menu {

    public class Lobby : MonoBehaviour
    {
		[SerializeField] List<PlayerSlotObject> playerSlots;

		private void Start() {

			PlayerManager playerManager = PlayerManager.Instance;
			playerManager.PlayerJoined += HandleOnPlayerJoined;
			playerManager.PlayerLeft += HandleOnPlayerLeft;
		}

		private void HandleOnPlayerJoined(PlayerComponent player) {

			var slots = playerSlots.OrderBy(x => x.Id).ToList();

			foreach (var slot in slots) {
				if (!slot.IsTaken) {

					slot.JoinPlayer(player);

					PlayerInputHandler playerInputHandler = player.GetComponent<PlayerInputHandler>();
					playerInputHandler.OnStart.AddListener(HandleOnStartGame);

					break;
				}
			}
		}

		private void HandleOnPlayerLeft(PlayerComponent player) {

			PlayerSlotObject playerSlot = playerSlots.FirstOrDefault(x => x.PlayerComponent.PlayerId == player.PlayerId);

			if (playerSlot == null)
				return;

			playerSlot.LeavePlayer();

			PlayerInputHandler playerInputHandler = player.GetComponent<PlayerInputHandler>();
			playerInputHandler.OnStart.RemoveListener(HandleOnStartGame);
		}

		private void HandleOnStartGame() {
			Scenes.SceneLoadManager.Instance.LoadGame();
		}
	}
}
