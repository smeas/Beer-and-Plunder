using System.Collections;
using UnityEngine;

namespace Audio {
	public class SoundHandle {
		public static SoundHandle NullHandle { get; } = new SoundHandle();

		private AudioSource source;
		private bool isValid;
		private Coroutine fadeRoutine;

		public SoundHandle() {
			isValid = false;
		}

		public SoundHandle(AudioSource source) {
			this.source = source;
			isValid = source != null;
		}

		/// <summary>
		/// The volume of the audio source bound to this handle.
		/// </summary>
		public float Volume {
			get => isValid ? source.volume : 1f;
			set {
				if (isValid)
					source.volume = value;
			}
		}

		/// <summary>
		/// Stop playing this sound.
		/// </summary>
		public void Stop() {
			if (!isValid) return;

			source.Stop();
			Invalidate();
		}

		/// <summary>
		/// Fade out, then stop this sound.
		/// </summary>
		/// <param name="fadeDuration">The duration of the fade.</param>
		public void FadeOutAndStop(float fadeDuration) {
			if (!isValid || fadeRoutine != null) return;
			fadeRoutine = AudioCoroutineHelper.Run(CoFadeOutAndStop(fadeDuration));
		}

		/// <summary>
		/// Invalidate the handle. (For internal use only.)
		/// </summary>
		public void Invalidate() {
			isValid = false;

			if (fadeRoutine != null) {
				AudioCoroutineHelper.Stop(fadeRoutine);
				fadeRoutine = null;
			}
		}

		private IEnumerator CoFadeOutAndStop(float duration) {
			float startVolume = source.volume;

			for (float time = duration; time >= 0f; time -= Time.unscaledDeltaTime) {
				if (!isValid)
					yield break;

				source.volume = time / duration * startVolume;
				yield return null;
			}

			fadeRoutine = null;
			Stop();
		}
	}
}