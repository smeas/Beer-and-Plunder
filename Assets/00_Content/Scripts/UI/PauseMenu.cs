using Rounds;
using Scenes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace UI {
	public class PauseMenu : SingletonBehaviour<PauseMenu> {
		[SerializeField] private InputActionProperty pauseAction;
		[SerializeField] private Selectable firstSelected;
		[SerializeField] private Canvas canvas;

		private bool isPaused;

		public bool IsPaused => isPaused;

		protected override void Awake() {
			base.Awake();

			canvas.gameObject.SetActive(false);
			pauseAction.action.Enable();
			pauseAction.action.performed += HandleOnPausePressed;
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			pauseAction.action.performed -= HandleOnPausePressed;
		}

		private void HandleOnPausePressed(InputAction.CallbackContext ctx) {
			if (RoundController.Instance != null && RoundController.Instance.IsRoundActive)
				TogglePaused();
		}

		public void TogglePaused() {
			isPaused = !isPaused;
			if (isPaused) {
				canvas.gameObject.SetActive(true);
				EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
				Time.timeScale = 0f;
			}
			else {
				canvas.gameObject.SetActive(false);
				Time.timeScale = 1f;
			}
		}

		// Unity event functions

		public void Resume() {
			TogglePaused();
		}

		public void Options() { /* TODO */ }

		public void ExitToMenu() {
			SceneLoadManager.Instance.LoadMainMenu();
		}
	}
}
