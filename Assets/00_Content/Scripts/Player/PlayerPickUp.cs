using System;
using Interactables;
using Rounds;
using UnityEngine;

namespace Player {

	public class PlayerPickUp : MonoBehaviour {

		[SerializeField] private Transform playerGrabTransform;
		[SerializeField] private GameObject playerRoot;

		private InteractionDetector detector;
		private PickUp pickedUpItem;
		private bool isUsingItem;

		public event Action<PickUp> OnItemPickedUp;
		public event Action<PickUp> OnItemDropped;

		public PickUp PickedUpItem => pickedUpItem;

		private void Awake() {
			detector = GetComponent<InteractionDetector>();

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += RespawnHeldItem;
		}

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= RespawnHeldItem;
		}

		public void PickUpClosestItem() {

			if (detector.ClosestPickUp == null)
				return;

			pickedUpItem = detector.ClosestPickUp;
			pickedUpItem.PickUpItem(playerGrabTransform);

			OnItemPickedUp?.Invoke(pickedUpItem);
		}

		// Run from unity event
		public void DropItem() {

			if (pickedUpItem == null)
				return;

			if (isUsingItem) {
				if (pickedUpItem is IUseable usable)
					usable.EndUse();

				isUsingItem = false;
			}

			pickedUpItem.DropItem(playerGrabTransform);
			OnItemDropped?.Invoke(pickedUpItem);

			pickedUpItem = null;

		}

		public void UseItem() {
			if (!(pickedUpItem is IUseable usable)) return;

			if (pickedUpItem != null) {
					usable.Use(playerRoot);
					isUsingItem = true;
			}
		}

		public void EndUseItem() {
			if (!(pickedUpItem is IUseable usable) || !isUsingItem) return;

			if (pickedUpItem != null) {
				usable.EndUse();
				isUsingItem = false;
			}
		}

		public bool CanPickUp(PickUp pickUp) {
			return pickedUpItem == null;
		}

		public void ConsumeItem() {
			if (pickedUpItem == null) return;

			Destroy(pickedUpItem.gameObject);
			pickedUpItem = null;
		}

		private void RespawnHeldItem() {
			if (pickedUpItem == null) return;

			PickUp item = pickedUpItem;

			DropItem();
			item.Respawn();
		}

		public void OnClosestPickUpChange(PickUp pickUp) { }
	}
}
