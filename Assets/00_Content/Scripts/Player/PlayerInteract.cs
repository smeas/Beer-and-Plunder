using Interactables;
using UnityEngine;

namespace Player {
	public class PlayerInteract : MonoBehaviour {
		private InteractionDetector detector;

		private void Start() {
			detector = GetComponent<InteractionDetector>();
		}

		// Called by unity event
		public void Interact() {
			if (detector.ClosestInteractable != null)
				detector.ClosestInteractable.Interact();
		}

		public bool CanInteract(Interactable interactable) {
			return true;
		}

		public void OnClosestInteractableChange(Interactable interactable) { }
	}
}