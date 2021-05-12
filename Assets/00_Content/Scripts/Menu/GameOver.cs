using Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
	[SerializeField] private TMP_Text gameOverMessage;
	[SerializeField] private string destructionMessage;
	[SerializeField] private string bankrupcyMessage;
	[SerializeField] private Selectable firstSelected;

	public void Show(LoseCondition loseCondition) {
		if (loseCondition == LoseCondition.Bankrupcy) gameOverMessage.text = bankrupcyMessage;
		else if (loseCondition == LoseCondition.Destruction) gameOverMessage.text = destructionMessage;

		gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
	}
	public void RestartGame() => SceneLoadManager.Instance.LoadLobby();
	public void GoToMainMenu() => SceneLoadManager.Instance.LoadMainMenu();
}