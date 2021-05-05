using Audio;
using UnityEditor;
using UnityEngine;

namespace Editors {
	[CustomEditor(typeof(SoundCue))]
	public class SoundCueEditor : Editor {
		public override void OnInspectorGUI() {
			serializedObject.Update();

			SerializedProperty modeProperty = serializedObject.FindProperty(nameof(SoundCue.mode));
			EditorGUILayout.PropertyField(modeProperty);

			if ((SoundCueMode)modeProperty.intValue == SoundCueMode.Single) {
				SerializedProperty singleClipProperty = serializedObject.FindProperty(nameof(SoundCue.singleClip));
				EditorGUILayout.PropertyField(singleClipProperty, new GUIContent("Audio Clip"));
			}
			else {
				SerializedProperty randomClipsProperty = serializedObject.FindProperty(nameof(SoundCue.randomClips));
				EditorGUILayout.PropertyField(randomClipsProperty, new GUIContent("Audio Clips"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}