using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities {
	public abstract class ExtendedEditor : Editor {
		private Dictionary<string, SerializedProperty> propertyCache;

		public override void OnInspectorGUI() {
			serializedObject.UpdateIfRequiredOrScript();
			DrawEditor();
			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void DrawEditor() {
			DrawAllProperties();
		}

		/// <summary>
		/// Get a copy of a property.
		/// </summary>
		protected SerializedProperty GetProperty(string propertyPath) {
			return GetPropertyRaw(propertyPath)?.Copy();
		}

		/// <summary>
		/// Get a property without copying it.
		/// </summary>
		protected SerializedProperty GetPropertyRaw(string propertyPath) {
			if (propertyCache == null) propertyCache = new Dictionary<string, SerializedProperty>();
			if (propertyCache.TryGetValue(propertyPath, out SerializedProperty property)) return property;

			property = serializedObject.FindProperty(propertyPath);
			if (property != null) propertyCache.Add(propertyPath, property);
			else
				Debug.LogError($"Property not found on serializedObject '{propertyPath}'");

			return property;
		}

		/// <summary>
		/// Draw a property.
		/// </summary>
		protected SerializedProperty DrawProperty(string propertyPath, bool includeChildren = true) {
			return DrawProperty(propertyPath, null, includeChildren);
		}

		/// <summary>
		/// Draw a property.
		/// </summary>
		protected SerializedProperty DrawProperty(string propertyPath, GUIContent label, bool includeChildren = true) {
			SerializedProperty property = GetProperty(propertyPath);
			if (property != null)
				DrawProperty(property, label, includeChildren);
			return property;
		}

		/// <summary>
		/// Draw a property. This method should not mutate the iterator.
		/// </summary>
		protected void DrawProperty(SerializedProperty property, bool includeChildren = true) {
			EditorGUILayout.PropertyField(property, includeChildren);
		}

		/// <summary>
		/// Draw a property. This method should not mutate the iterator.
		/// </summary>
		protected void DrawProperty(SerializedProperty property, GUIContent label, bool includeChildren = true) {
			EditorGUILayout.PropertyField(property, label, includeChildren);
		}

		/// <summary>
		/// Draw all properties.
		/// </summary>
		protected void DrawAllProperties(bool includeChildren = true) {
			foreach (SerializedProperty property in serializedObject.GetTopLevelProperties(true, true))
				DrawProperty(property, includeChildren);
		}

		/// <summary>
		/// Draw properties, starting at the specified one.
		/// </summary>
		protected void DrawProperties(string propertyPath, bool includeChildren = true) {
			SerializedProperty property = GetProperty(propertyPath);
			if (property != null)
				DrawPropertiesRaw(property, null, includeChildren);
		}

		/// <summary>
		/// Draw properties, starting at 'property' and optionally ending at 'last'.
		/// </summary>
		protected void DrawProperties(SerializedProperty property, string last = null, bool includeChildren = true) {
			DrawPropertiesRaw(property.Copy(), last, includeChildren);
		}

		/// <summary>
		/// Draw properties, starting at 'property', and optionally ending at 'last' without first copying the property.
		/// </summary>
		protected void DrawPropertiesRaw(SerializedProperty property, string last = null, bool includeChildren = true) {
			do {
				DrawProperty(property, includeChildren);
				if (last != null && property.name == last)
					break;
			} while (property.NextVisible(false));
		}


		protected IEnumerable<SerializedProperty> GetProperties(bool skipScript = true) {
			return serializedObject.GetTopLevelProperties(skipScript);
		}

		protected IEnumerable<SerializedProperty> GetPropertiesRaw(bool skipScript = true) {
			return serializedObject.GetTopLevelProperties(skipScript, true);
		}
	}
}