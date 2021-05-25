using Audio;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Menu {
	public class SettingsMenu : MonoBehaviour {
		[SerializeField] private DefaultSettingsData defaultSettings;

		[Space]
		[SerializeField] private Slider masterSlider;
		[SerializeField] private Slider musicSlider;
		[SerializeField] private Slider sfxSlider;

		private void Start() {
			masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultSettings.masterVolume);
			musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultSettings.musicVolume);
			sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultSettings.sfxVolume);
		}

		private void OnEnable() {
			masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
			musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
			sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
		}

		private void OnDisable() {
			masterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
			musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
			sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);

			PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
			PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
			PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
		}

		private void OnMasterSliderChanged(float value) {
			AudioManager.Instance.Volume = value;
		}

		private void OnMusicSliderChanged(float value) {
			AudioManager.Instance.MusicVolume = value;
		}

		private void OnSFXSliderChanged(float value) {
			AudioManager.Instance.EffectsVolume = value;
		}
	}
}
