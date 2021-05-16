using Scenes;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Menu {
	public class MenuController : MonoBehaviour {
		[SerializeField] private GameObject mainMenuPanel;
		[SerializeField] private GameObject settingsPanel;
		[SerializeField] private GameObject startGameButton;

		[Header("Timeline")]
		[SerializeField] private PlayableDirector timelineDirector;
		[SerializeField] private TimelineAsset toLobbyTimeline;

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
			EventSystem.current.SetSelectedGameObject(null);
			timelineDirector.playableAsset = toLobbyTimeline;
			timelineDirector.Play();
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

		// Run from animation event
		public void SelectStartGame() {
			EventSystem.current.SetSelectedGameObject(startGameButton);
		}
	}
}
