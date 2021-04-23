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

		public void GoToSettings() {
			mainMenuPanel.SetActive(false);
			settingsPanel.SetActive(true);
		}

		public void ReturnToMainMenu() {
			settingsPanel.SetActive(false);
			mainMenuPanel.SetActive(true);
		}

		public void QuitGame() {
			Application.Quit();
			Debug.Log("Trying to quit game.");
		}
	}
}