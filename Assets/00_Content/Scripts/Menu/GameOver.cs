using Player;
using Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

	public void RestartGame() {
		foreach (PlayerComponent player in PlayerManager.Instance.Players) {
			PlayerInput playerInput = player.GetComponent<PlayerInput>();
			playerInput.SwitchCurrentActionMap("Game");
		}

		SceneLoadManager.Instance.LoadGame();
	}

	public void GoToMainMenu() => SceneLoadManager.Instance.LoadMainMenu();
}
