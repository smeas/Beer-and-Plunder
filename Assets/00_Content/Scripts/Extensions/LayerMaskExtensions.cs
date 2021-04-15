using UnityEngine;

namespace Extensions {
	public static class LayerMaskExtensions {
		/// <summary>
		/// Returns true if the layer mask contains the specified layer.
		/// </summary>
		public static bool ContainsLayer(this LayerMask layerMask, int layerIndex) {
			return ((1 << layerIndex) & layerMask) != 0;
		}
	}
}