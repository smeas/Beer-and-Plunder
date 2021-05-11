using UnityEngine;

namespace Audio {
	public class UIAudioHelper : MonoBehaviour {
		public void PlaySoundEffect(SoundEffect effect) {
			AudioManager.PlayEffectSafe(effect);
		}

		public void PlaySoundEffect(SoundCue cue) {
			AudioManager.PlayEffectSafe(cue);
		}
	}
}