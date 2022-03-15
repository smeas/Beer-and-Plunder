using System.Collections.Generic;
using Player;
using Scenes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Menu {
	public class MenuController : MonoBehaviour {
		[SerializeField] private GameObject startPanel;
		[SerializeField] private GameObject backButton;
		[SerializeField] private GameObject feedbackButton;

		[Space]
		[SerializeField] private GameObject startGameButton;
		[SerializeField] private ParticleSystem clickEffect;

		[Space]
		[SerializeField] private InputActionProperty backAction;

		[Header("Timeline")]
		[SerializeField] private PlayableDirector timelineDirector;
		[SerializeField] private TimelineAsset toLobbyTimeline;

		private Stack<(GameObject panel, GameObject selection)> panels = new Stack<(GameObject panel, GameObject selection)>();
		private GameObject currentPanel;
		private bool transitionActive;

		private void Start() {
			if (PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players) {
					PlayerInput playerInput = player.GetComponent<PlayerInput>();
					playerInput.SwitchCurrentActionMap("UI");
				}
			}

			currentPanel = startPanel;

			startPanel.SetActive(true);
			backButton.SetActive(false);

			backAction.action.Enable();
			backAction.action.performed += OnBackActionPerformed;
		}

		private void OnDestroy() {
			backAction.action.performed -= OnBackActionPerformed;
		}

		private void OnBackActionPerformed(InputAction.CallbackContext _) {
			ClosePanel();
		}

		public void GoToLobby() {
			if (transitionActive) return;

			EventSystem.current.SetSelectedGameObject(null);
			timelineDirector.playableAsset = toLobbyTimeline;
			timelineDirector.Play();
			transitionActive = true;
		}

		public void GoToTutorial() {
			if (transitionActive) return;

			SceneLoadManager.Instance.LoadTutorial();
			transitionActive = true;
		}

		public void ShowPanel(GameObject panel) {
			if (transitionActive) return;

			clickEffect.Play();

			currentPanel.SetActive(false);

			panels.Push((currentPanel, EventSystem.current.currentSelectedGameObject));

			panel.SetActive(true);
			currentPanel = panel;

			if (panels.Count > 0) {
				backButton.SetActive(true);
				feedbackButton.SetActive(false);
			}
		}

		public void ClosePanel() {
			if (transitionActive) return;
			if (panels.Count == 0) return;

			clickEffect.Play();

			currentPanel.SetActive(false);

			(GameObject panel, GameObject selection) = panels.Pop();
			panel.SetActive(true);
			currentPanel = panel;

			EventSystem.current.SetSelectedGameObject(selection);

			if (panels.Count == 0) {
				backButton.SetActive(false);
				feedbackButton.SetActive(true);
			}
		}

		public void OpenFeedBackSurvey() {
			Application.OpenURL("https://forms.gle/JRiHycVZS6LwuDf37");
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
			transitionActive = false;
		}
	}
}
