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
		
        private PlayerManager playerManager;

		private void Start() {
			playerManager = PlayerManager.Instance;
			playerManager.PlayerJoined += HandleOnPlayerJoined;
			playerManager.PlayerLeft += HandleOnPlayerLeft;
		}

		private void HandleOnPlayerJoined(PlayerComponent player) {

			var slots = playerSlots.OrderBy(x => x.Id).ToList();

			foreach (var slot in slots) {
				if (!slot.IsTaken) {

					slot.JoinPlayer(player);

					var playerInput = player.GetComponent<PlayerInputHandler>();
					playerInput.OnStart.AddListener(HandleOnStartGame);

					break;
				}
			}
		}

		private void HandleOnPlayerLeft(PlayerComponent player) {

			var playerSlot = playerSlots.FirstOrDefault(x => x.PlayerComponent.PlayerId == player.PlayerId);

			if (playerSlot == null)
				return;

			playerSlot.LeavePlayer();

			var playerInput = player.GetComponent<PlayerInputHandler>();
			playerInput.OnStart.RemoveListener(HandleOnStartGame);
		}

		private void HandleOnStartGame() {
			Scenes.SceneLoadManager.Instance.LoadGame();
		}
	}
}
