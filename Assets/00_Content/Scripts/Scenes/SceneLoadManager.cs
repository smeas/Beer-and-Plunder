using System;
using Audio;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Scenes {
	public class SceneLoadManager : SingletonBehaviour<SceneLoadManager> {
		[SerializeField] private SceneTransition sceneTransitionPrefab;

		[Header("Scenes")]
		[SerializeField] private SceneInfo mainMenu;
		[SerializeField] private SceneInfo tutorial;
		[SerializeField] private SceneInfo game;

		private SceneTransition transition;

		public SceneInfo CurrentScene {
			get {
				int currentIndex = SceneManager.GetActiveScene().buildIndex;
				if (currentIndex == mainMenu.scene.BuildIndex)
					return mainMenu;
				if (currentIndex == tutorial.scene.BuildIndex)
					return tutorial;
				if (currentIndex == game.scene.BuildIndex)
					return game;

				return null;
			}
		}

		protected override void Awake() {
			base.Awake();

			transition = Instantiate(sceneTransitionPrefab, transform);
			transition.gameObject.SetActive(false);

			DontDestroyOnLoad(gameObject);

			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			// Move the players back out of the DDOL scene
			if (mode == LoadSceneMode.Single && PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players)
					SceneManager.MoveGameObjectToScene(player.gameObject, scene);
			}
		}

		public void LoadMainMenu() {
			Load(mainMenu, true, () => {
				// Remove all player when going back to main menu
				if (PlayerManager.Instance != null) {
					for (int i = PlayerManager.Instance.NumPlayers - 1; i >= 0; i--)
						Destroy(PlayerManager.Instance.Players[i].gameObject);
				}
			});
		}

		public void LoadTutorial() => Load(tutorial, true);
		public void LoadGame() => Load(game, true);

		private void Load(SceneInfo sceneInfo, bool doTransition, Action beforeEnterScene = null) {
			// Make sure players are carried over
			if (PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players)
					DontDestroyOnLoad(player.gameObject);
			}

			SceneInfo currentScene = CurrentScene;
			MusicCue nextMusic = sceneInfo.music;
			bool switchMusic = nextMusic != currentScene?.music;

			void BeforeEnterScene() {
				beforeEnterScene?.Invoke();

				if (switchMusic)
					AudioManager.PlayMusicSafe(nextMusic, MusicFadeType.LinearOutIn, transition.TransitionDuration / 2f);

				// Reset time scale when loading a new scene
				Time.timeScale = 1f;
			}

			if (doTransition) {
				if (switchMusic)
					AudioManager.StopMusicSafe(transition.TransitionDuration / 2f);

				transition.Play(sceneInfo, BeforeEnterScene);
			}
			else {
				// FIXME: How do we fade here?
				if (switchMusic)
					AudioManager.PlayMusicSafe(nextMusic, MusicFadeType.LinearOutIn, 0.4f, restart: true);

				BeforeEnterScene();
				sceneInfo.Load();
			}
		}


		// Kicks off the music when the game is started
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnInitialize() {
			if (Instance == null)
				return;

			SceneInfo currentScene = Instance.CurrentScene;
			if (currentScene != null && currentScene.music != null) {
				AudioManager.PlayMusicSafe(currentScene.music);
			}
		}
	}
}
