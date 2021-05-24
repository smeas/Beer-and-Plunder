using DG.Tweening;
using Rounds;
using Taverns;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	[SerializeField] private Image clockBackgroundDark;
	[SerializeField] private Transform clockPointer;
	[SerializeField] private Image moneyFillBar;
	[SerializeField] private TMP_Text moneyCurrentText;
	[SerializeField] private TMP_Text moneyRequiredText;
	[SerializeField] private TMP_Text roundStatusText;

	[Header("Effects")]
	[SerializeField] private float moneyPunchStrength = 1.25f;
	[SerializeField] private float moneyCurrentPunchDuration = 0.2f;

	void Start() {
		if (Tavern.Instance) {
			HandleOnMoneyChanges();
			moneyCurrentText.transform.DOKill();
			Tavern.Instance.OnMoneyChanges += HandleOnMoneyChanges;
		} else {
			Debug.Log("The HUD cannot find an instance of the Tavern-singleton while in the Start-function.");
		}

		if (RoundController.Instance != null) {
			RoundController.Instance.OnIntermissionStart += HandleOnIntermissionStart;
			RoundController.Instance.OnRoundOver += HandleOnRoundOver;
		}
	}

	void Update() {
		UpdateClock();
	}

	private void UpdateClock() {
		if (RoundController.Instance != null) {
			float progress = RoundController.Instance.RoundTimer / RoundController.Instance.RoundDuration;
			progress = Mathf.Clamp01(progress);

			clockBackgroundDark.fillAmount = 1 - progress;
			clockPointer.localEulerAngles = new Vector3(0, 0, -progress * 360);
		}
	}

	private void HandleOnMoneyChanges() {
		int requiredMoney = RoundController.Instance != null ? RoundController.Instance.RequiredMoney : 0;

		moneyFillBar.fillAmount = (float)Tavern.Instance.Money / requiredMoney;

		moneyCurrentText.text = Tavern.Instance.Money.ToString();
		moneyCurrentText.transform.DOKill();
		moneyCurrentText.transform.localScale = Vector3.one;
		moneyCurrentText.transform.DOPunchScale(new Vector2(moneyPunchStrength, moneyPunchStrength), moneyCurrentPunchDuration);

		moneyRequiredText.text = requiredMoney.ToString();
	}

	private void HandleOnIntermissionStart() {
		roundStatusText.enabled = true;
		roundStatusText.text = "Intermission";
	}

	private void HandleOnRoundOver() {
		roundStatusText.enabled = false;
	}
}
