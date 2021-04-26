using System.Collections;
using System.Collections.Generic;
using Scenes;
using UnityEngine;

public class GameOver : MonoBehaviour
{
	//TODO: Go through this again,
	public void RestartGame() => SceneLoadManager.Instance.LoadLobby();
	public void GoToMainMenu() => SceneLoadManager.Instance.LoadMainMenu();
}