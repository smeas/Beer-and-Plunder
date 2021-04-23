using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Scenes {
	public class SceneLoadManager : SingletonBehaviour<SceneLoadManager> {
		[SerializeField] private SceneInfo mainMenu;
		[SerializeField] private SceneInfo game;
		[SerializeField] private SceneInfo lobby;

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

			DontDestroyOnLoad(gameObject);
		}

		public void LoadLobby() {
			lobby.Load();
		}

		public void LoadMainMenu() {
			mainMenu.Load();
		}

		public void LoadGame() {
			game.Load();
		}
	}
}
