using System.Collections;
using System.Collections.Generic;
using Player;
using Scenes;
using UnityEngine;

public class GameOver : MonoBehaviour {
	public void RestartGame() => SceneLoadManager.Instance.LoadLobby();
	public void GoToMainMenu() => SceneLoadManager.Instance.LoadMainMenu();
}