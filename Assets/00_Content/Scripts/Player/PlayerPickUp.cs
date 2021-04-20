using Interactables;
using UnityEngine;

namespace Player {

	public class PlayerPickUp : MonoBehaviour {

		[SerializeField] private Transform playerGrabTransform;
		[SerializeField] private GameObject playerRoot;

		private InteractionDetector detector;
		private PickUp pickedUpItem;
		private bool isUsingItem;

		public PickUp PickedUpItem => pickedUpItem;

		private void Start() {
			detector = GetComponent<InteractionDetector>();
		}

		public void PickUpClosestItem() {

			if (detector.ClosestPickUp == null)
				return;

			pickedUpItem = detector.ClosestPickUp;
			pickedUpItem.PickUpItem(playerGrabTransform);
		}

		public void DropItem() {

			if (pickedUpItem == null)
				return;

			if (isUsingItem) {
				if (pickedUpItem is IUseable usable)
					usable.EndUse();

				isUsingItem = false;
			}

			pickedUpItem.DropItem();
			pickedUpItem = null;
		}

		public void UseItem() {
			if (pickedUpItem != null && pickedUpItem is IUseable usable) {
				usable.Use(playerRoot);
				isUsingItem = true;
			}
		}

		public void EndUseItem() {
			if (pickedUpItem != null && pickedUpItem is IUseable usable) {
				usable.EndUse();
				isUsingItem = false;
			}
		}

		public bool CanPickUp(PickUp pickUp) {
			return pickedUpItem == null;
		}

		public void OnClosestPickUpChange(PickUp pickUp) { }
	}
}
