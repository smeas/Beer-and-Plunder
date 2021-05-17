using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	public class SettingsMenu : MonoBehaviour {
		[SerializeField] private Slider masterSlider;
		[SerializeField] private Slider musicSlider;
		[SerializeField] private Slider sfxSlider;

		[Header("DefaultValues")]
		[SerializeField] private float masterVolume = 0.5f;
		[SerializeField] private float musicVolume = 0.5f;
		[SerializeField] private float sfxVolume = 0.5f;

		private void Start() {
			masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", masterVolume);
			musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
			sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
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

			PlayerPrefs.SetFloat("MasterVolume", masterVolume);
			PlayerPrefs.SetFloat("MusicVolume", musicVolume);
			PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
		}

		private void OnMasterSliderChanged(float value) {
			AudioManager.Instance.Volume = value;
			masterVolume = value;
		}

		private void OnMusicSliderChanged(float value) {
			AudioManager.Instance.MusicVolume = value;
			musicVolume = value;
		}

		private void OnSFXSliderChanged(float value) {
			AudioManager.Instance.EffectsVolume = value;
			sfxVolume = value;
		}
	}
}
