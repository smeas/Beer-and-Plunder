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
		private AudioSource musicIntroSource;
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

			musicIntroSource = Instantiate(musicSourcePrefab, transform);
			musicIntroSource.name = "Music Intro Source";

			musicSource = Instantiate(musicSourcePrefab, transform);
			musicSource.name = "Music Source";
		}

		public SoundHandle PlayEffect(SoundEffect effect, bool loop = false) {
			SoundCue cue = GetSoundEffect(effect);
			return PlayEffect(cue.GetClip(), loop, cue.volume, cue.pitch);
		}

		public SoundHandle PlayEffect(SoundCue cue, bool loop = false) {
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
		public void PlayMusic(AudioClip musicClip, FadeKind fade = FadeKind.NoFade, float fadeDuration = 0f,
		                      bool restart = false, bool loop = true) {
			PlayMusic(null, musicClip, fade, fadeDuration, restart, loop);
		}

		public void PlayMusic(MusicCue cue, FadeKind fade = FadeKind.NoFade, float fadeDuration = 0f,
		                      bool restart = false, bool loop = true) {
			PlayMusic(cue.introClip, cue.mainClip, fade, fadeDuration, restart, loop);
		}

		private void PlayMusic(AudioClip introClip, AudioClip musicClip, FadeKind fade = FadeKind.NoFade,
		                       float fadeDuration = 0f, bool restart = false, bool loop = true) {
			if (!restart
				&& musicIntroSource.clip == introClip && musicSource.clip == musicClip
				&& (musicIntroSource.isPlaying || musicSource.isPlaying)) return;

			StopFadingMusic();

			switch (fade) {
				case FadeKind.OutIn:
					StartCoroutine(CoFadeMusicOutIn(introClip, musicClip, fadeDuration, loop));
					break;
				case FadeKind.In:
					musicIntroSource.Stop();
					musicSource.Stop();
					StartCoroutine(CoFadeMusicOutIn(introClip, musicClip, fadeDuration, loop));
					break;
				default:
					musicIntroSource.Stop();
					musicSource.Stop();
					PlayMusicNowInternal(introClip, musicClip, loop, 1f);
					break;
			}
		}

		public void StopMusic(float fadeDuration = 0f) {
			StopFadingMusic();

			if (fadeDuration > 0f) {
				StartCoroutine(CoFadeMusicOut(fadeDuration));
			}
			else {
				musicIntroSource.Stop();
				musicSource.Stop();
			}
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

		private IEnumerator CoFadeMusicOutIn(AudioClip newIntroClip, AudioClip newMusicClip, float duration, bool loop) {
			bool alreadyPlaying = musicSource.isPlaying || musicIntroSource.isPlaying;

			if (alreadyPlaying) {
				yield return CoFadeMusicOut(duration / 2f);
			}

			// Start new track
			PlayMusicNowInternal(newIntroClip, newMusicClip, loop, 0f);

			yield return CoFadeMusicIn(alreadyPlaying ? duration / 2f : duration);
		}

		private IEnumerator CoFadeMusicOut(float duration) {
			for (float time = duration; time >= 0f; time -= Time.unscaledDeltaTime) {
				float volume = time / duration;
				musicIntroSource.volume = volume;
				musicSource.volume = volume;

				yield return null;
			}

			musicIntroSource.Stop();
			musicSource.Stop();
		}

		private IEnumerator CoFadeMusicIn(float duration) {
			for (float time = 0f; time < duration; time += Time.unscaledDeltaTime) {
				float volume = time / duration;
				musicIntroSource.volume = volume;
				musicSource.volume = volume;

				yield return null;
			}
		}

		private void PlayMusicNowInternal(AudioClip introClip, AudioClip musicClip, bool loop, float startVolume) {
			musicIntroSource.clip = introClip;
			musicIntroSource.volume = startVolume;

			musicSource.clip = musicClip;
			musicSource.loop = loop;
			musicSource.volume = startVolume;

			if (introClip != null) {
				musicIntroSource.Play();
				musicSource.PlayDelayed(introClip.length);
			}
			else {
				musicSource.Play();
			}
		}


		#region Static wrappers

		public static SoundHandle PlayEffectSafe(SoundEffect effect, bool loop = false) {
			if (Instance != null)
				return Instance.PlayEffect(effect, loop);

			return SoundHandle.NullHandle;
		}

		public static SoundHandle PlayEffectSafe(SoundCue cue, bool loop = false) {
			if (Instance != null)
				return Instance.PlayEffect(cue, loop);

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

		public static void PlayMusicSafe(MusicCue cue, FadeKind fade = FadeKind.NoFade, float fadeDuration = 0f,
		                                 bool restart = false, bool loop = true) {
			if (Instance != null)
				Instance.PlayMusic(cue.introClip, cue.mainClip, fade, fadeDuration, restart, loop);
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