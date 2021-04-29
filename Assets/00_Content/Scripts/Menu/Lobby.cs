using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu {

    public class Lobby : MonoBehaviour {
		[SerializeField] List<PlayerSlotObject> playerSlots;
		[SerializeField] private GameObject[] playerModels;
		[SerializeField] private Color[] playerColors;

		private PlayerManager playerManager;

		private void Start() {

			playerManager = PlayerManager.Instance;
			playerManager.PlayerJoined += HandleOnPlayerJoined;
			playerManager.PlayerLeft += HandleOnPlayerLeft;

			if (playerModels.Length < PlayerManager.MaxPlayers)
				Debug.LogError("Not enough player models for everyone!", this);

			if (playerColors.Length < PlayerManager.MaxPlayers)
				Debug.LogError("Not enough player colors", this);
		}

		private void HandleOnPlayerJoined(PlayerComponent player) {
			List<PlayerSlotObject> slots = playerSlots.OrderBy(x => x.Id).ToList();

			for (int i = 0; i < slots.Count; i++) {
				PlayerSlotObject slot = slots[i];
				if (!slot.IsTaken) {
					slot.JoinPlayer(player);

					PlayerInputHandler playerInputHandler = player.GetComponent<PlayerInputHandler>();
					playerInputHandler.OnStart.AddListener(HandleOnStartGame);
					player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");

					// Give the player a unique model
					Debug.Assert(player.ModelRoot.childCount == 1, "Model root does not have exactly one child", player.ModelRoot);
					Destroy(player.ModelRoot.GetChild(0).gameObject);
					player.BodyMeshRenderer = Instantiate(playerModels[i], player.ModelRoot).GetComponent<MeshRenderer>();

					foreach (Material material in player.BodyMeshRenderer.materials)
						material.color = playerColors[i];

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

			playerManager.PlayerJoined -= HandleOnPlayerJoined;
			playerManager.PlayerLeft -= HandleOnPlayerLeft;

			foreach (PlayerSlotObject playerSlot in playerSlots) {
				if (playerSlot.IsTaken) {
					PlayerInputHandler playerInputHandler = playerSlot.PlayerComponent.GetComponent<PlayerInputHandler>();
					playerInputHandler.transform.position = Vector3.zero;
					playerInputHandler.OnStart.RemoveListener(HandleOnStartGame);
					playerSlot.PlayerComponent.GetComponent<PlayerInput>().SwitchCurrentActionMap("Game");
				}
			}

			playerManager.AllowJoining = false;

			Scenes.SceneLoadManager.Instance.LoadGame();
		}
	}
}
