using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu {
	public class PlayerSlotObject : MonoBehaviour {
		[SerializeField] private Vector2 offset;
		[SerializeField] private GameObject joinText;
		[SerializeField] private Image inputTypeImage;

		[Space]
		[SerializeField] private Sprite keyboardIcon;
		[SerializeField] private Sprite gamepadIcon;

		public int Id;
		public bool IsTaken => isTaken;
		public PlayerComponent PlayerComponent => playerComponent;

		private bool isTaken;
		private PlayerComponent playerComponent;

		private void Start() {
			StartCoroutine(CoDelayJoinText());
		}

		// In update due to it positioning badly the first frame
		private void Update() {
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(PlayerSpawnController.Instance.SpawnPoints[Id-1].position);
			transform.position = screenPoint + new Vector3(offset.x, offset.y, 0f);
		}

		// Delay frames to prevent flickering of text
		private IEnumerator CoDelayJoinText() {
			joinText.SetActive(false);
			const int framesToWait = 1;

			for (int i = 0; i < framesToWait; i++)
				yield return null;

			joinText.SetActive(!isTaken);
		}

		public void JoinPlayer(PlayerComponent player) {
			if (isTaken)
				return;

			joinText.SetActive(false);
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
			joinText.SetActive(true);
			inputTypeImage.gameObject.SetActive(false);

			isTaken = false;
			playerComponent = null;

			StopAllCoroutines();
		}
	}
}
