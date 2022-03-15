using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnalyticsManager : MonoBehaviour {
	[SerializeField] private GameObject infoPanel;
	[SerializeField] private GameObject firstSelected;

	private List<string> requiredConsents = new List<string>();

	private async void Start() {
		infoPanel.SetActive(false);
#if UNITY_EDITOR
		EventSystem.current.SetSelectedGameObject(firstSelected);
#else
		await UnityServices.InitializeAsync();
		bool consentChecked = PlayerPrefs.GetInt("ConsentChecked", 0) > 0;

		try {
			requiredConsents = await Events.CheckForRequiredConsents();

			if (requiredConsents.Count == 0 && consentChecked)
				EventSystem.current.SetSelectedGameObject(firstSelected);
			else
				infoPanel.SetActive(true);
		}
		catch (ConsentCheckException) {
			OptOut();
		}
#endif
	}

	public void Consent() {
#if !UNITY_EDITOR
		foreach (string identifier in requiredConsents)
			Events.ProvideOptInConsent(identifier, true);
#endif
		PlayerPrefs.SetInt("ConsentChecked", 1);
	}

	public void OptOut() {
#if !UNITY_EDITOR
		foreach (string identifier in requiredConsents)
			Events.ProvideOptInConsent(identifier, false);

		Events.OptOut();
#endif
		PlayerPrefs.SetInt("ConsentChecked", 1);
	}

	public void OpenPrivacyURL() {
		Application.OpenURL(Events.PrivacyUrl);
	}
}
