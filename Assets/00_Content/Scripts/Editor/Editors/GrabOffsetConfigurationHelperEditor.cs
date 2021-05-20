using Player;
using UnityEditor;
using UnityEngine;

namespace Editors {
	[CustomEditor(typeof(GrabOffsetConfigurationHelper))]
	public class GrabOffsetConfigurationHelperEditor : Editor {
		public override void OnInspectorGUI() {
			serializedObject.UpdateIfRequiredOrScript();

			SerializedProperty activeProperty = serializedObject.FindProperty("active");

			using (new EditorGUI.DisabledScope(!Application.isPlaying)) {
				bool active = activeProperty.boolValue;
				if (GUILayout.Button(active ? "Deactivate" : "Activate"))
					activeProperty.boolValue = !active;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}