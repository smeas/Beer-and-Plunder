using Scenes;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using UnityEngine.UI;
using TMPro;

namespace Menu {
	public class MenuController : MonoBehaviour {
		public GameObject mainMenuPanel;
		public GameObject settingsPanel;

		private void Start() {

			if (PlayerManager.Instance != null) {

				foreach (PlayerComponent player in PlayerManager.Instance.Players) {
					PlayerInput playerInput = player.GetComponent<PlayerInput>();
					playerInput.SwitchCurrentActionMap("UI");
				}
			}
			settingsPanel.SetActive(false);
			mainMenuPanel.SetActive(true);
		}

		public void GoToLobby() {
			mainMenuPanel.SetActive(false);
			settingsPanel.SetActive(false);

			SceneLoadManager.Instance.LoadLobby();
		}

		public void GoToTutorial() {
			SceneLoadManager.Instance.LoadTutorial();
		}

		public void GoToSettings() {
			mainMenuPanel.SetActive(false);
			settingsPanel.SetActive(true);
		}

		public void ReturnToMainMenu() {
			settingsPanel.SetActive(false);
			mainMenuPanel.SetActive(true);
		}

		public void QuitGame() {
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.ExitPlaymode();
			#else
				Application.Quit();
			#endif
		}
	}
}