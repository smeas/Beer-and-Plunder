using Audio;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editors {
	[CustomEditor(typeof(SoundCue))]
	public class SoundCueEditor : ExtendedEditor {
		protected override void DrawEditor() {
			SoundCueMode mode = (SoundCueMode)DrawProperty(nameof(SoundCue.mode)).intValue;

			if (mode == SoundCueMode.Single)
				DrawProperty(nameof(SoundCue.singleClip), new GUIContent("Audio Clip"));
			else
				DrawProperty(nameof(SoundCue.randomClips), new GUIContent("Audio Clips"));

			DrawProperties(nameof(SoundCue.volume));
		}
	}
}