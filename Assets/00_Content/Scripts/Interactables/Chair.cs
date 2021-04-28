using UnityEngine;
using Vikings;

namespace Interactables {
	public class Chair : MonoBehaviour {
		[SerializeField] private Transform sitPoint;
		[SerializeField] private Transform dismountPoint;

		public Table Table { get; set; }
		public Viking SittingViking { get; private set; }
		public bool IsOccupied => SittingViking != null;
		public Transform SitPivot => sitPoint != null ? sitPoint : transform;
		public Transform DismountPoint => dismountPoint;

		public void OnVikingTakeChair(Viking viking) {
			if (SittingViking != null)
				Debug.LogError("A viking is attempting to take an already taken chair", this);

			SittingViking = viking;
			viking.IsSeated = true;
		}

		public void OnVikingLeaveChair(Viking viking) {
			if (viking != SittingViking) {
				Debug.LogError("The viking leaving a chair is not the same as the one sitting", this);
				return;
			}

			SittingViking = null;
			viking.IsSeated = false;
		}
	}
}
