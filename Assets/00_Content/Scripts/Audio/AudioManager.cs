using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;

namespace Audio {
	[DefaultExecutionOrder(-100)]
	public class AudioManager : SingletonBehaviour<AudioManager> {
		[SerializeField] private AudioSource effectSourcePrefab;
		[SerializeField] private AudioSource musicSourcePrefab;
		[SerializeField] private AudioMixer mixer;
		[SerializeField] private int effectVoiceCount = 16;

		private AudioSourcePool soundEffectPool;
		private AudioSource musicSource;

		private Dictionary<string, SoundCue> soundEffectCache = new Dictionary<string, SoundCue>();
		private Coroutine musicFadeRoutine;

		public float Volume {
			get => mixer.GetFloat("MasterVolume", out float value) ? MathX.DecibelsToLinear(value) : 1f;
			set => mixer.SetFloat("MasterVolume", MathX.LinearToDecibels(value));
		}

		public float MusicVolume {
			get => mixer.GetFloat("MusicVolume", out float value) ? MathX.DecibelsToLinear(value) : 1f;
			set => mixer.SetFloat("MusicVolume", MathX.LinearToDecibels(value));
		}

		public float EffectsVolume {
			get => mixer.GetFloat("EffectsVolume", out float value) ? MathX.DecibelsToLinear(value) : 1f;
			set => mixer.SetFloat("EffectsVolume", MathX.LinearToDecibels(value));
		}

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);

			soundEffectPool = new AudioSourcePool(effectSourcePrefab, effectVoiceCount, transform, "Sound Effects");
			musicSource = Instantiate(musicSourcePrefab, transform);
			musicSource.name = "Music Source";
		}

		public SoundHandle PlayEffect(SoundEffect effect, bool loop = false) {
			SoundCue cue = GetSoundEffect(effect);
			return PlayEffect(cue.GetClip(), loop, cue.volume, cue.pitch);
		}

		public SoundHandle PlayEffect(AudioClip clip, bool loop = false, float volume = 1f, float pitch = 1f) {
			return soundEffectPool.PlayOneShot(clip, loop, volume, pitch);
		}

		/// <summary>
		/// Play a music track.
		/// </summary>
		/// <param name="musicClip">The track to play.</param>
		/// <param name="fade">The type of fade to use.</param>
		/// <param name="fadeDuration">The duration of the fade.</param>
		/// <param name="restart">Whether to restart playback if the same track is already playing.</param>
		/// <param name="loop">Whether to loop the track.</param>
		public void PlayMusic(AudioClip musicClip, FadeKind fade = FadeKind.NoFade, float fadeDuration = 0f, bool restart = false, bool loop = true) {
			if (!restart && musicSource.clip == musicClip && musicSource.isPlaying)
				return;

			// If we're already fading, stop it.
			if (musicFadeRoutine != null) {
				StopCoroutine(musicFadeRoutine);
				musicFadeRoutine = null;
			}

			if (fade == FadeKind.OutIn) {
				musicFadeRoutine = StartCoroutine(CoFadeMusicOutIn(musicClip, fadeDuration, loop));
			}
			else {
				musicSource.clip = musicClip;
				musicSource.loop = loop;
				musicSource.volume = 1f;
				musicSource.Play();
			}
		}

		public SoundCue GetSoundEffect(SoundEffect effect) {
			string effectPath = AudioIndex.GetPath(effect);
			SoundCue cue;

			if (soundEffectCache.TryGetValue(effectPath, out cue)) {
				return cue;
			}
			else {
				cue = Resources.Load<SoundCue>(effectPath);
				Debug.Assert(cue != null, "Loaded sound cue was null");
				soundEffectCache.Add(effectPath, cue);
				return cue;
			}
		}

		private IEnumerator CoFadeMusicOutIn(AudioClip newMusicClip, float duration, bool loop) {
			float halfDuration = duration / 2f;

			// Fade out if needed
			if (musicSource.isPlaying) {
				for (float time = halfDuration; time >= 0f; time -= Time.unscaledDeltaTime) {
					musicSource.volume = time / halfDuration;
					yield return null;
				}
			}

			// Start new track
			musicSource.clip = newMusicClip;
			musicSource.loop = loop;
			musicSource.volume = 0f;
			musicSource.Play();

			// Fade in
			for (float time = 0f; time < halfDuration; time += Time.unscaledDeltaTime) {
				musicSource.volume = time / halfDuration;
				yield return null;
			}

			musicFadeRoutine = null;
		}


		// Static usefulness

		public static SoundHandle PlayEffectSafe(SoundEffect effect, bool loop = false) {
			if (Instance != null)
				return Instance.PlayEffect(effect, loop);

			return SoundHandle.NullHandle;
		}

		public static SoundHandle PlayEffectSafe(AudioClip clip, bool loop = false, float volume = 1f, float pitch = 1f) {
			if (Instance != null)
				return Instance.PlayEffect(clip, loop, volume, pitch);

			return SoundHandle.NullHandle;
		}

		public static void PlayMusicSafe(AudioClip musicClip, FadeKind fade = FadeKind.NoFade, float fadeDuration = 0f,
		                             bool restart = false, bool loop = true) {
			if (Instance != null)
				Instance.PlayMusic(musicClip, fade, fadeDuration, restart, loop);
		}
	}

	public enum FadeKind {
		NoFade,
		OutIn,
	}
}