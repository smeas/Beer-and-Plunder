using System.Collections.Generic;
using UnityEditor;

namespace Utilities {
	public static class SerializedObjectExtensions {
		public static IEnumerable<SerializedProperty> GetTopLevelProperties(
			this SerializedObject serializedObject, bool skipScript = true, bool copy = true) {
			SerializedProperty property = serializedObject.GetIterator();
			if (!property.NextVisible(true)) yield break;
			do {
				if (skipScript && property.name == "m_Script") {
					skipScript = false;
					continue;
				}

				yield return copy ? property.Copy() : property;
			} while (property.NextVisible(false));
		}

		public static SerializedProperty GetFirstProperty(this SerializedObject serializedObject) {
			SerializedProperty property = serializedObject.GetIterator();
			return property.NextVisible(true) ? property : null;
		}

		public static IEnumerable<SerializedProperty> GetFollowingProperties(
			this SerializedProperty property, bool copy = true) {
			property = copy ? property.Copy() : property;
			do
				yield return copy ? property.Copy() : property;
			while (property.NextVisible(false));
		}

		public static IEnumerable<SerializedProperty> GetFollowingProperties(
			this SerializedProperty property, string stopAt, bool copy = true) {
			property = copy ? property.Copy() : property;
			do {
				yield return copy ? property.Copy() : property;
				if (property.name == stopAt) yield break;
			} while (property.NextVisible(false));
		}
	}
}