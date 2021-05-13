using UnityEngine;

namespace Audio {
	[CreateAssetMenu(fileName = "Music Cue", menuName = "Game/Audio/Music Cue")]
	public class MusicCue : ScriptableObject {
		public AudioClip introClip;
		public AudioClip mainClip;
	}
}