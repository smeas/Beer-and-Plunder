using DG.Tweening;
using Player;
using Scenes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
	[SerializeField] private Image image;
	[SerializeField] private Sprite bankruptSprite;
	[SerializeField] private Sprite destructionSprite;

	[Header("Settings")]
	[SerializeField] private float signEnterDuration;

	private RectTransform imageRect;

	private void Awake() {
		imageRect = image.GetComponent<RectTransform>();
	}

	public void Show(LoseCondition loseCondition) {
		image.sprite = loseCondition switch {
			LoseCondition.Bankrupcy => bankruptSprite,
			LoseCondition.Destruction => destructionSprite,
			_ => image.sprite
		};

		Vector3 correctPosition = imageRect.anchoredPosition;
		imageRect.anchoredPosition = Vector3.zero;
		imageRect.DOAnchorPos(correctPosition, signEnterDuration).SetEase(Ease.OutBounce);

		gameObject.SetActive(true);
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
