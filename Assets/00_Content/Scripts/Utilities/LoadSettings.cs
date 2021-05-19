using Audio;
using UnityEngine;

namespace Utilities {
	public class LoadSettings : MonoBehaviour {
		[SerializeField] private DefaultSettingsData defaultSettings;

		void Start() {
			AudioManager.Instance.Volume = PlayerPrefs.GetFloat("MasterVolume", defaultSettings.masterVolume);
			AudioManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultSettings.musicVolume);
			AudioManager.Instance.EffectsVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSettings.sfxVolume);
		}
	}
}
