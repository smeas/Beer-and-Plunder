using UnityEngine;

namespace Vikings {
	public class DesireVisualiser : MonoBehaviour {
		private GameObject visualisation;

		public void ShowNewDesire(GameObject desireVisualisationPrefab) {
			if (visualisation == null) Destroy(visualisation);

			if (desireVisualisationPrefab == null) {
				Debug.Assert(false, "Desire visualisation is null");
				return;
			}

			visualisation = Instantiate(desireVisualisationPrefab, transform);
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
