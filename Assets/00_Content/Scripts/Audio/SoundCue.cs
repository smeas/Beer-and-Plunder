using UnityEngine;

namespace Audio {
	[CreateAssetMenu(fileName = "SoundCue", menuName = "Game/Audio/Sound Cue")]
	public class SoundCue : ScriptableObject {
		public SoundCueMode mode;
		public AudioClip singleClip;
		public AudioClip[] randomClips;

		public AudioClip GetClip() {
			if (mode == SoundCueMode.Single)
				return singleClip;
			else
				return randomClips[Random.Range(0, randomClips.Length)];
		}
	}

	public enum SoundCueMode {
		Single,
		Random
	}
}