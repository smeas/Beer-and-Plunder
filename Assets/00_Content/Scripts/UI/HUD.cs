using DG.Tweening;
using Rounds;
using Taverns;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class HUD : MonoBehaviour {
	[SerializeField] private TMP_Text tavernMoneyText;

	[SerializeField] private Image clockBackgroundDark;
	[SerializeField] private Transform clockPointer;
	[SerializeField] private Image moneyFillBar;
	[SerializeField] private TMP_Text moneyCurrentText;
	[SerializeField] private TMP_Text moneyRequiredText;

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
			tavernMoneyText.text = " ";
		}
	}

	void Update() {
		UpdateClock();
	}

	private void UpdateClock() {
		if (RoundController.Instance != null) {
			float progress = RoundController.Instance.RoundTimer / RoundController.Instance.RoundDuration;

			clockBackgroundDark.fillAmount = 1 - progress;
			clockPointer.localEulerAngles = new Vector3(0, 0, -MathX.RemapClamped(progress, 0, 1, 0, 360));
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
}
