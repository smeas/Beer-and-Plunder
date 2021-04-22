using UnityEngine;
using UnityEngine.UI;
using Rounds;
using Taverns;
using TMPro;

public class HUDScript : MonoBehaviour {
	[SerializeField] private Slider tavernHealthSlider;
	[SerializeField] private TMP_Text tavernMoneyText;
	[SerializeField] private TMP_Text roundTimerText;

	void Start() {
		if (Tavern.Instance) {
			tavernHealthSlider.maxValue = Tavern.Instance.MaxHealth;
			tavernMoneyText.text = Tavern.Instance.StartingMoney.ToString();
			tavernHealthSlider.value = Tavern.Instance.StartingHealth;

			Tavern.Instance.OnHealthChanges += HandleOnHealthChanges;
			Tavern.Instance.OnMoneyChanges += HandleOnMoneyChanges;
		} else {
			Debug.Log("The HUD cannot find an instance of the Tavern-singleton while in the Start-function.");
			tavernMoneyText.text = " ";
			tavernHealthSlider.value = 0;
		}
    }

	void Update() {
		roundTimerText.text = Mathf.RoundToInt(RoundController.Instance.RoundTimer).ToString();
	}

	private void HandleOnHealthChanges() {
		tavernHealthSlider.value = Tavern.Instance.Health;
	}

	private void HandleOnMoneyChanges() {
		tavernMoneyText.text = Mathf.RoundToInt(Tavern.Instance.Money).ToString();
	}
}