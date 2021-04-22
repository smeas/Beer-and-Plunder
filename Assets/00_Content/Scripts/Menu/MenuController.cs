using Scenes;
using UnityEngine;

namespace Menu {
	public class MenuController : MonoBehaviour {

		//So, this will later on have some way of turning on and of what input to be using yeah?

		public void GoToLobby() {
			SceneLoadManager.Instance.LoadLobby();
		}

		public void GoToSettings() {
			//this one can activate another panel for settings on top of it.
		}

		public void QuitGame() {
			Application.Quit();
			Debug.Log("Trying to quit game.");
		}
	}
}
