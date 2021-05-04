using System.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu {
	public class PlayerSlotObject : MonoBehaviour {
		[SerializeField] private Image background;
		[SerializeField] private TextMeshProUGUI joinText;
		[SerializeField] private TextMeshProUGUI readyText;
		[SerializeField] private Image inputTypeImage;

		[Space]
		[SerializeField] private float flashTime = 0.08f;
		[SerializeField] private float flashAlpha = 0.5f;
		[SerializeField] private Sprite keyboardIcon;
		[SerializeField] private Sprite gamepadIcon;

		public int Id;
		public bool IsTaken => isTaken;
		public PlayerComponent PlayerComponent => playerComponent;

		private bool isTaken;
		private PlayerComponent playerComponent;
		private Color defaultBackgroundColor;

		private void Start() {
			defaultBackgroundColor = background.color;
		}

		public void JoinPlayer(PlayerComponent player) {
			if (isTaken)
				return;

			readyText.gameObject.SetActive(true);
			joinText.gameObject.SetActive(false);
			inputTypeImage.gameObject.SetActive(true);

			background.color = player.PlayerColor;
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
			readyText.gameObject.SetActive(false);
			joinText.gameObject.SetActive(true);
			inputTypeImage.gameObject.SetActive(false);

			background.color = defaultBackgroundColor;
			isTaken = false;
			playerComponent = null;

			StopAllCoroutines();
		}

		public void Flash() {
			StartCoroutine(CoFlash());
		}

		private IEnumerator CoFlash() {
			Color bg = background.color;
			background.color = new Color(bg.r, bg.g, bg.b, flashAlpha);
			yield return new WaitForSeconds(flashTime);
			background.color = bg;
		}
	}
}