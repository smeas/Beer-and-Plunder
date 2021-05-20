using UnityEngine;

namespace Extensions {
	public static class TransformExtensions {
		/// <summary>
		/// Performs a depth first search for a child transform with the specified name.
		/// </summary>
		public static Transform FindChildByNameRecursive(this Transform transform, string name) {
			foreach (Transform child in transform) {
				if (child.name == name)
					return child;

				Transform t = child.FindChildByNameRecursive(name);
				if (t != null)
					return t;
			}

			return null;
		}
	}
}