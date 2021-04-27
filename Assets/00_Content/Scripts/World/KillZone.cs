using UnityEngine;

namespace World {
	public class KillZone : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
			if (other.isTrigger)
				return;

			other.GetComponentInParent<IRespawnable>()?.Respawn();
		}
	}
}