using System.Collections.Generic;
using System.Linq;
using Player;
using Scenes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu {

	public class Lobby : MonoBehaviour {
		[SerializeField] private List<PlayerSlotObject> playerSlots;
		[SerializeField] private GameObject[] playerModels;
		[SerializeField] private Color[] playerColors;
		[SerializeField] private InputActionProperty backAction;
		[SerializeField] private ReadySystem readySystem;

		private PlayerManager playerManager;

		private void OnEnable() {

			playerManager = PlayerManager.Instance;

			foreach (PlayerComponent player in playerManager.Players) {
				HandleOnPlayerJoined(player);
			}

			playerManager.PlayerJoined += HandleOnPlayerJoined;
			playerManager.PlayerLeft += HandleOnPlayerLeft;

			if (playerModels.Length < PlayerManager.MaxPlayers)
				Debug.LogError("Not enough player models for everyone!", this);

			if (playerColors.Length < PlayerManager.MaxPlayers)
				Debug.LogError("Not enough player colors", this);

			backAction.action.Enable();
			backAction.action.started += HandleOnBackPressed;

			playerManager.AllowJoining = true;

			readySystem.Initialize();
			readySystem.AllReady += HandleOnStartGame;
		}

		private void HandleOnPlayerJoined(PlayerComponent player) {
			List<PlayerSlotObject> slots = playerSlots.OrderBy(x => x.Id).ToList();

			for (int i = 0; i < slots.Count; i++) {
				PlayerSlotObject slot = slots[i];
				if (!slot.IsTaken) {
					player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");

					// Give the player a unique model and color
					player.SwitchCharacterModel(playerModels[i]);
					player.PlayerColor = playerColors[i];

					slot.JoinPlayer(player);

					break;
				}
			}
		}

		private void HandleOnPlayerLeft(PlayerComponent player) {

			PlayerSlotObject playerSlot = FindPlayerSlot(player);

			if (playerSlot == null)
				return;

			playerSlot.LeavePlayer();
		}

		private void HandleOnStartGame() {

			foreach (PlayerSlotObject playerSlot in playerSlots) {
				if (playerSlot.IsTaken)
					playerSlot.PlayerComponent.GetComponent<PlayerInput>().SwitchCurrentActionMap("Game");
			}

			OnLeaveMenu();
			SceneLoadManager.Instance.LoadGame();
		}

		private void OnLeaveMenu() {
			backAction.action.started -= HandleOnBackPressed;

			playerManager.PlayerJoined -= HandleOnPlayerJoined;
			playerManager.PlayerLeft -= HandleOnPlayerLeft;

			readySystem.AllReady -= HandleOnStartGame;

			playerManager.AllowJoining = false;
		}

		private void HandleOnBackPressed(InputAction.CallbackContext ctx) {
			// Find out what player pressed back
			PlayerComponent player = PlayerManager.Instance.Players.FirstOrDefault(plr => {
				PlayerInput playerInput = plr.GetComponent<PlayerInput>();
				return playerInput.devices.Contains(ctx.control.device);
			});

			// If the player is joined, leave it. Otherwise go back to the main menu.
			if (player != null)
				LeavePlayer(player);
			else
				BackToMenu();
		}

		private void BackToMenu() {
			// Leave all players
			List<PlayerComponent> players = PlayerManager.Instance.Players;
			for (int i = players.Count - 1; i >= 0; i--)
				LeavePlayer(players[i]);

			OnLeaveMenu();
			SceneLoadManager.Instance.LoadMainMenu();
		}

		private void LeavePlayer(PlayerComponent player) {
			Destroy(player.gameObject);
		}

		private PlayerSlotObject FindPlayerSlot(PlayerComponent player) {
			return playerSlots.FirstOrDefault(x => x.PlayerComponent == player);
		}
	}
}
