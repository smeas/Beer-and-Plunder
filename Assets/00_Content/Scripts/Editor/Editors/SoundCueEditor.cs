using Audio;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editors {
	[CustomEditor(typeof(SoundCue))]
	public class SoundCueEditor : ExtendedEditor {
		protected override void DrawEditor() {
			DrawProperty(nameof(SoundCue.audioClips), new GUIContent("Audio Clips"));
			DrawProperties(nameof(SoundCue.volume));
		}
	}
}