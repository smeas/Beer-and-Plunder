using UnityEngine;

namespace Interactables {
	public class Interactable : MonoBehaviour {
		[SerializeField] public Transform highlightPivot;

		public virtual void CancelInteraction(GameObject player, PickUp item) {
		}
		public virtual void Interact(GameObject player, PickUp item) {
		}

		public virtual bool CanInteract(GameObject player, PickUp item) {
			return true;
		}
	}
}
