using UnityEngine;
using Vikings;

namespace Interactables {
	public class Chair : MonoBehaviour {
		[SerializeField] private Transform sitPoint;

		public Viking SittingViking { get; set; }
		public Transform SitPivot => sitPoint != null ? sitPoint : transform;

		public void OnVikingTakeChair(Viking viking) {
			if (SittingViking != null)
				Debug.LogError("A viking is attempting to take an already taken chair", this);

			SittingViking = viking;
		}

		public void OnVikingLeaveChair(Viking viking) {
			if (viking != SittingViking) {
				Debug.LogError("The viking leaving a chair is not the same as the one sitting", this);
				return;
			}

			SittingViking = null;
		}
	}
}