using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu {
	public class PlayerSlotObject : MonoBehaviour {
		[SerializeField] private Vector2 offset;
		[SerializeField] private TextMeshProUGUI joinText;
		[SerializeField] private Image inputTypeImage;

		[Space]
		[SerializeField] private Sprite keyboardIcon;
		[SerializeField] private Sprite gamepadIcon;

		public int Id;
		public bool IsTaken => isTaken;
		public PlayerComponent PlayerComponent => playerComponent;

		private bool isTaken;
		private PlayerComponent playerComponent;

		private void OnEnable() {
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(PlayerSpawnController.Instance.SpawnPoints[Id-1].position);
			transform.position = screenPoint + new Vector3(offset.x, offset.y, 0f);
		}

		public void JoinPlayer(PlayerComponent player) {
			if (isTaken)
				return;

			joinText.gameObject.SetActive(false);
			inputTypeImage.gameObject.SetActive(true);

			inputTypeImage.color = player.PlayerColor;
			isTaken = true;

			playerComponent = player;

			PlayerInput playerInput = player.GetComponent<PlayerInput>();
			inputTypeImage.sprite = playerInput.currentControlScheme switch {
				"KeyboardMouse" => keyboardIcon,
				"Gamepad" => gamepadIcon,
				_ => null
			};
		}

		public void LeavePlayer() {
			joinText.gameObject.SetActive(true);
			inputTypeImage.gameObject.SetActive(false);

			isTaken = false;
			playerComponent = null;

			StopAllCoroutines();
		}
	}
}
