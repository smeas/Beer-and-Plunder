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

		private Dictionary<SoundEffect, SoundCue> soundEffectCache = new Dictionary<SoundEffect, SoundCue>();

		/// <summary>
		/// Linear master volume.
		/// </summary>
		public float Volume {
			get => mixer.GetFloat("MasterVolume", out float value) ? MathX.DecibelsToLinear(value) : 1f;
			set => mixer.SetFloat("MasterVolume", MathX.LinearToDecibels(value));
		}

		/// <summary>
		/// Linear music volume.
		/// </summary>
		public float MusicVolume {
			get => mixer.GetFloat("MusicVolume", out float value) ? MathX.DecibelsToLinear(value) : 1f;
			set => mixer.SetFloat("MusicVolume", MathX.LinearToDecibels(value));
		}

		/// <summary>
		/// Linear effects volume.
		/// </summary>
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

			StopFadingMusic();

			switch (fade) {
				case FadeKind.OutIn:
					StartCoroutine(CoFadeMusicOutIn(musicClip, fadeDuration, loop));
					break;
				case FadeKind.In:
					musicSource.Stop();
					StartCoroutine(CoFadeMusicOutIn(musicClip, fadeDuration, loop));
					break;
				default:
					musicSource.clip = musicClip;
					musicSource.loop = loop;
					musicSource.volume = 1f;
					musicSource.Play();
					break;
			}
		}

		public void StopMusic() {
			StopFadingMusic();
			musicSource.Stop();
		}

		public void FadeOutAndStopMusic(float duration) {
			StopFadingMusic();
			StartCoroutine(CoFadeMusicOut(duration));
		}

		public SoundCue GetSoundEffect(SoundEffect effect) {
			if (soundEffectCache.TryGetValue(effect, out SoundCue cue)) {
				return cue;
			}
			else {
				string effectPath = AudioIndex.GetPath(effect);
				cue = Resources.Load<SoundCue>(effectPath);
				Debug.Assert(cue != null, "Loaded sound cue was null");

				soundEffectCache.Add(effect, cue);
				return cue;
			}
		}

		private void StopFadingMusic() {
			StopAllCoroutines();
		}

		private IEnumerator CoFadeMusicOut(float duration) {
			for (float time = duration; time >= 0f; time -= Time.unscaledDeltaTime) {
				musicSource.volume = time / duration;
				yield return null;
			}

			musicSource.Stop();
		}

		private IEnumerator CoFadeMusicIn(float duration) {
			for (float time = 0f; time < duration; time += Time.unscaledDeltaTime) {
				musicSource.volume = time / duration;
				yield return null;
			}
		}

		private IEnumerator CoFadeMusicOutIn(AudioClip newMusicClip, float duration, bool loop) {
			bool alreadyPlaying = musicSource.isPlaying;

			if (alreadyPlaying) {
				yield return CoFadeMusicOut(duration / 2f);
			}

			// Start new track
			musicSource.clip = newMusicClip;
			musicSource.loop = loop;
			musicSource.volume = 0f;
			musicSource.Play();

			yield return CoFadeMusicIn(alreadyPlaying ? duration / 2f : duration);
		}


		#region Static wrappers

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

		/// <inheritdoc cref="PlayMusic"/>
		public static void PlayMusicSafe(AudioClip musicClip, FadeKind fade = FadeKind.NoFade, float fadeDuration = 0f,
		                                 bool restart = false, bool loop = true) {
			if (Instance != null)
				Instance.PlayMusic(musicClip, fade, fadeDuration, restart, loop);
		}

		public static void StopMusicSafe() {
			if (Instance != null)
				Instance.StopMusic();
		}

		public static void FadeOutAndStopMusicSafe(float duration) {
			if (Instance != null)
				Instance.FadeOutAndStopMusic(duration);
		}

		#endregion
	}

	public enum FadeKind {
		NoFade,
		OutIn,
		In,
	}
}