using System.Collections.Generic;
using Scenes;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Menu {
	public class MenuController : MonoBehaviour {
		[SerializeField] private GameObject startPanel;
		[SerializeField] private GameObject backButton;

		[Space]
		[SerializeField] private GameObject startGameButton;

		[Header("Timeline")]
		[SerializeField] private PlayableDirector timelineDirector;
		[SerializeField] private TimelineAsset toLobbyTimeline;

		private Stack<GameObject> panels = new Stack<GameObject>();

		private void Start() {
			if (PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players) {
					PlayerInput playerInput = player.GetComponent<PlayerInput>();
					playerInput.SwitchCurrentActionMap("UI");
				}
			}

			startPanel.SetActive(true);
			panels.Push(startPanel);
			backButton.SetActive(false);
		}

		public void GoToLobby() {
			EventSystem.current.SetSelectedGameObject(null);
			timelineDirector.playableAsset = toLobbyTimeline;
			timelineDirector.Play();
		}

		public void GoToTutorial() {
			SceneLoadManager.Instance.LoadTutorial();
		}

		public void ShowPanel(GameObject panel) {
			panels.Peek().SetActive(false);
			panel.SetActive(true);
			panels.Push(panel);

			if (panels.Count > 1)
				backButton.SetActive(true);
		}

		public void ClosePanel() {
			if (panels.Count == 1) return;

			panels.Pop().SetActive(false);
			panels.Peek().SetActive(true);

			if (panels.Count == 1)
				backButton.SetActive(false);
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
