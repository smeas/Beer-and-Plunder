using UnityEngine;

namespace Audio {
	public class UIAudioHelper : MonoBehaviour {
		public void PlaySoundEffect(SoundEffect effect) {
			AudioManager.PlayEffectSafe(effect);
		}
	}
}