using Rounds;
using Taverns;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	[SerializeField] private TMP_Text tavernMoneyText;
	[SerializeField] private TMP_Text roundTimerText;

	void Start() {
		if (Tavern.Instance) {
			HandleOnMoneyChanges();
			Tavern.Instance.OnMoneyChanges += HandleOnMoneyChanges;
		} else {
			Debug.Log("The HUD cannot find an instance of the Tavern-singleton while in the Start-function.");
			tavernMoneyText.text = " ";
		}
	}

	void Update() {
		if (RoundController.Instance != null)
			roundTimerText.text = Mathf.RoundToInt(RoundController.Instance.RoundTimer).ToString();
	}

	private void HandleOnMoneyChanges() {
		int requiredMoney = RoundController.Instance != null ? RoundController.Instance.RequiredMoney : 0;
		tavernMoneyText.text = $"{Mathf.RoundToInt(Tavern.Instance.Money)}/{requiredMoney}";
	}
}
