using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Scenes {
	public class SceneLoadManager : SingletonBehaviour<SceneLoadManager> {
		[SerializeField] private SceneTransition sceneTransitionPrefab;

		[Header("Scenes")]
		[SerializeField] private SceneInfo mainMenu;
		[SerializeField] private SceneInfo game;
		[SerializeField] private SceneInfo lobby;

		private SceneTransition transition;

		public SceneInfo CurrentScene {
			get {
				int currentIndex = SceneManager.GetActiveScene().buildIndex;
				if (currentIndex == mainMenu.scene.BuildIndex)
					return mainMenu;
				if (currentIndex == game.scene.BuildIndex)
					return game;
				if (currentIndex == lobby.scene.BuildIndex)
					return lobby;

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

		public void LoadLobby() => Load(lobby, false);
		public void LoadMainMenu() => Load(mainMenu, true);
		public void LoadGame() => Load(game, true);

		private void Load(SceneInfo sceneInfo, bool doTransition) {
			// Make sure players are carried over
			if (PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players)
					DontDestroyOnLoad(player.gameObject);
			}

			if (doTransition)
				transition.Play(sceneInfo);
			else
				sceneInfo.Load();
		}
	}
}
