using UnityEngine;

namespace Interactables {
	public class Interactable : MonoBehaviour {

		public virtual void CancelInteraction(GameObject player, PickUp item) {
		}
		public virtual void Interact(GameObject player, PickUp item) {
		}

		public virtual bool CanInteract(GameObject player, PickUp item) {
			return true;
		}
	}
}
