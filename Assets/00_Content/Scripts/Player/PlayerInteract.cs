using Interactables;
using UnityEngine;

namespace Player {
	public class PlayerInteract : MonoBehaviour {
		[SerializeField] private GameObject playerRoot;

		private InteractionDetector detector;
		private PlayerPickUp playerPickUp;
		private PlayerSteward playerSteward;
		private bool isInteracting;

		private Interactable currentInteractable;

		public bool IsInteracting => isInteracting;

		private void Awake() {
			detector = GetComponent<InteractionDetector>();
			playerPickUp = GetComponent<PlayerPickUp>();
			playerSteward = playerRoot.GetComponent<PlayerSteward>();
		}

		public void Interact() {
			if (detector.ClosestInteractable == null)
				return;

			if (playerSteward.Follower != null && detector.ClosestInteractable is Table table)
				TryTakeSeatForViking(table);
			else {
				if (CanInteract(detector.ClosestInteractable)) {
					detector.ClosestInteractable.Interact(playerRoot, playerPickUp.PickedUpItem);
					currentInteractable = detector.ClosestInteractable;
					isInteracting = true;
				}
			}
		}

		public void EndInteract() {
			if (isInteracting && currentInteractable != null) {
				currentInteractable.CancelInteraction(playerRoot, playerPickUp.PickedUpItem);
				currentInteractable = null;
				isInteracting = false;
			}
		}

		private void TryTakeSeatForViking(Table table) {
			if (table.TryFindEmptyChairForViking(playerSteward.Follower, out Chair chair)) {
				bool success = playerSteward.Follower.TryTakeSeat(chair);
				Debug.Assert(success, "Follower viking accepted a seat");
			}
		}

		public bool CanInteract(Interactable interactable) {
			return interactable.CanInteract(playerRoot, playerPickUp.PickedUpItem);
		}

		public void OnClosestInteractableChange(Interactable interactable) { }
	}
}
