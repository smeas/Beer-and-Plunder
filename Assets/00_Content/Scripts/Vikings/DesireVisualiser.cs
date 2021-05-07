using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Vikings {
	public class DesireVisualiser : MonoBehaviour {
		[SerializeField] private Color color = Color.green;

		private GameObject visualisation;

		public void ShowNewDesire(GameObject desireVisualisationPrefab) {
			if (visualisation == null) Destroy(visualisation);

			if (desireVisualisationPrefab == null) {
				Debug.Assert(false, "Desire visualisation is null");
				return;
			}

			visualisation = Instantiate(desireVisualisationPrefab, transform);

			// TODO: Delete this
			MeshRenderer[] meshRenderers = visualisation.GetComponentsInChildren<MeshRenderer>();
			foreach (Material material in meshRenderers.SelectMany(x => x.materials))
				material.color = color;

			foreach (MeshRenderer meshRenderer in meshRenderers)
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		}

		public void HideDesire() {
			if (visualisation == null) {
				Debug.Assert(false, "Trying to remove a non-existing desire visualisation");
				return;
			}

			Destroy(visualisation);
		}
	}
}
