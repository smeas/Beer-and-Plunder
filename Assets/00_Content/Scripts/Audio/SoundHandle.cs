using UnityEngine;

namespace Audio {
	public class SoundHandle {
		private AudioSource source;
		private bool isValid;

		public SoundHandle() {
			isValid = false;
		}

		public SoundHandle(AudioSource source) {
			this.source = source;
			isValid = source != null;
		}

		public void Stop() {
			if (!isValid) return;

			source.Stop();
			Invalidate();
		}

		public void Invalidate() {
			isValid = false;
		}
	}
}