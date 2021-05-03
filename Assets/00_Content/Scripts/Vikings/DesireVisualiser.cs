using System.Linq;
using UnityEngine;

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
			foreach (Material material in visualisation.GetComponentsInChildren<MeshRenderer>().SelectMany(x => x.materials))
				material.color = color;
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
