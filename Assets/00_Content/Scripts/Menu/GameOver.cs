using System.Collections;
using System.Collections.Generic;
using Scenes;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour {
	[SerializeField] private TMP_Text gameOverMessage;

	[SerializeField] private string destructionMessage;
	[SerializeField] private string bankrupcyMessage;

	public void Show(LoseCondition loseCondition) {
		if (loseCondition == LoseCondition.Bankrupcy) gameOverMessage.text = bankrupcyMessage;
		else if (loseCondition == LoseCondition.Destruction) gameOverMessage.text = destructionMessage;

		gameObject.SetActive(true);
	}
	public void RestartGame() => SceneLoadManager.Instance.LoadLobby();
	public void GoToMainMenu() => SceneLoadManager.Instance.LoadMainMenu();
}