using Scenes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Menu {
	public class MenuController : MonoBehaviour {
		public GameObject mainMenuPanel;
		public GameObject settingsPanel;

		private void Start() {
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
