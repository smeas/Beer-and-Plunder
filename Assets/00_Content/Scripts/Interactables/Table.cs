using UnityEngine;
using Vikings;

namespace Interactables {
	public class Table : Interactable {
		private Chair[] chairs;

		private void Start() {
			chairs = GetComponentsInChildren<Chair>();
		}

		public bool TryFindEmptyChairForViking(Viking viking, out Chair closest) {
			Vector3 vikingPosition = viking.transform.position;

			closest = null;
			float minDistance = float.PositiveInfinity;

			foreach (Chair chair in chairs) {
				if (chair.SittingViking != null)
					continue;

				float distance = (vikingPosition - chair.SitPivot.position).sqrMagnitude;
				if (distance < minDistance) {
					minDistance = distance;
					closest = chair;
				}
			}

			return closest != null;
		}
	}
}