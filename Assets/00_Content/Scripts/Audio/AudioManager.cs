using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Audio {
	public class AudioManager : SingletonBehaviour<AudioManager> {
		[SerializeField] private AudioSource effectSourcePrefab;
		[SerializeField] private AudioSource musicSourcePrefab;
		[SerializeField] private int effectVoiceCount = 16;

		private AudioSourcePool soundEffectPool;
		private AudioSource musicSource;

		private Dictionary<string, SoundCue> soundEffectCache = new Dictionary<string, SoundCue>();

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);

			soundEffectPool = new AudioSourcePool(effectSourcePrefab, effectVoiceCount, transform, "Sound Effects");
			musicSource = Instantiate(musicSourcePrefab, transform);
			musicSource.name = "Music Source";
		}

		public void PlayEffectOneShot(SoundEffect effect) {
			SoundCue cue = LoadEffect(effect);
			PlayEffectOneShot(cue.GetClip());
		}

		public void PlayEffectOneShot(AudioClip clip) {
			PlayEffectOneShot(clip, Vector3.zero);
		}

		public void PlayEffectOneShot(AudioClip clip, Vector3 position) {
			soundEffectPool.PlayOneShot(clip, position);
		}

		public void PlayMusic(AudioClip musicClip, bool restart = false, bool loop = true) {
			if (!restart && musicSource.clip == musicClip && musicSource.isPlaying)
				return;

			musicSource.clip = musicClip;
			musicSource.loop = loop;
			musicSource.Play();
		}

		private SoundCue LoadEffect(SoundEffect effect) {
			string effectPath = AudioIndex.GetPath(effect);
			SoundCue cue;

			if (soundEffectCache.TryGetValue(effectPath, out cue)) {
				return cue;
			}
			else {
				cue = Resources.Load<SoundCue>(effectPath);
				soundEffectCache.Add(effectPath, cue);
				return cue;
			}
		}
	}
}