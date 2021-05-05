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

		public float Volume {
			get => isValid ? source.volume : 1f;
			set {
				if (isValid)
					source.volume = value;
			}
		}

		public void Stop() {
			if (!isValid) return;

			source.Stop();
			Invalidate();
		}

		public void FadeOutAndStop(float fadeDuration) {
			if (!isValid || fadeRoutine != null) return;
			fadeRoutine = AudioCoroutineHelper.Run(CoFadeOutAndStop(fadeDuration));
		}

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