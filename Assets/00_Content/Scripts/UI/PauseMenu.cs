using Rounds;
using Scenes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utilities;

namespace UI {
	public class PauseMenu : SingletonBehaviour<PauseMenu> {
		[SerializeField] private InputActionProperty pauseAction;
		[SerializeField] private InputActionProperty backAction;

		[SerializeField] private Canvas canvas;

		[SerializeField] private GameObject pausePanel;
		[SerializeField] private GameObject settingsPanel;

		[SerializeField] private GameObject optionsButton;

		private bool isPaused;

		public bool IsPaused => isPaused;

		protected override void Awake() {
			base.Awake();

			canvas.gameObject.SetActive(false);
			pauseAction.action.Enable();
			pauseAction.action.performed += HandleOnPausePressed;

			backAction.action.Enable();
			backAction.action.performed += HandleOnBackPressed;
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			pauseAction.action.performed -= HandleOnPausePressed;
			backAction.action.performed -= HandleOnBackPressed;
		}

		private void HandleOnBackPressed(InputAction.CallbackContext ctx) {
			HandleOnPausePressed(ctx);
		}

		private void HandleOnPausePressed(InputAction.CallbackContext ctx) {
			if (RoundController.Instance != null && !RoundController.Instance.IsRoundActive) return;

			if (settingsPanel.activeSelf) {
				ExitOptions();
				return;
			}

			TogglePaused();
		}

		public void TogglePaused() {
			isPaused = !isPaused;
			if (isPaused) {
				if (settingsPanel.activeSelf) ExitOptions();
				canvas.gameObject.SetActive(true);
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

		public void Options() {
			pausePanel.SetActive(false);
			settingsPanel.SetActive(true);
		}

		public void ExitOptions() {
			settingsPanel.SetActive(false);
			pausePanel.SetActive(true);
			EventSystem.current.SetSelectedGameObject(optionsButton);
		}

		public void ExitToMenu() {
			SceneLoadManager.Instance.LoadMainMenu();
		}
	}
}
