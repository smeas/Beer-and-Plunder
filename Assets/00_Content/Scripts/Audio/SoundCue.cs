using UnityEngine;
using UnityEngine.Serialization;

namespace Audio {
	[CreateAssetMenu(fileName = "SoundCue", menuName = "Game/Audio/Sound Cue")]
	public class SoundCue : ScriptableObject {
		[FormerlySerializedAs("randomClips")]
		public AudioClip[] audioClips;

		[Space]
		[Range(0, 1)]
		public float volume = 1f;
		[Range(-3, 3)]
		public float pitch = 1f;

		/// <summary>
		/// Get a clip to play.
		/// </summary>
		public AudioClip GetClip() {
			if (audioClips.Length == 0)
				return null;
			if (audioClips.Length == 1)
				return audioClips[0];

			return audioClips[Random.Range(0, audioClips.Length)];
		}
	}
}