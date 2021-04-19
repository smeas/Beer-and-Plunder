using Interactables;
using UnityEngine;

namespace Player {
	public class PlayerInteract : MonoBehaviour {
		[SerializeField] private GameObject playerRoot;

		private InteractionDetector detector;
		private PlayerPickUp playerPickUp;
		private PlayerSteward playerSteward;

		private void Start() {
			detector = GetComponent<InteractionDetector>();
			playerPickUp = GetComponent<PlayerPickUp>();
			playerSteward = playerRoot.GetComponent<PlayerSteward>();
		}

		// Called by unity event
		public void Interact() {
			if (detector.ClosestInteractable == null)
				return;

			if (playerSteward.Follower != null && detector.ClosestInteractable is Table table)
				TryTakeSeatForViking(table);
			else
				detector.ClosestInteractable.Interact(playerRoot, playerPickUp.PickedUpItem);
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