using System;
using Interactables;
using UnityEngine;

namespace Player {

	public class PlayerPickUp : MonoBehaviour {

		[SerializeField] private Transform playerGrabTransform;
		[SerializeField] private GameObject playerRoot;
		[SerializeField] private float throwStrengthMultiplier = 1f;

		private PlayerMovement playerMovement;
		private InteractionDetector detector;
		private PickUp pickedUpItem;
		private bool isUsingItem;

		public event Action<PickUp> OnItemPickedUp;
		public event Action<PickUp> OnItemDropped;
		public Transform PlayerGrabTransform => playerGrabTransform;

		public PickUp PickedUpItem => pickedUpItem;
		public bool IsUsingItem => isUsingItem;

		private void Awake() {
			detector = GetComponent<InteractionDetector>();
			playerMovement = GetComponentInParent<PlayerMovement>();
		}

		public void PickUpClosestItem() {

			if (detector.ClosestPickUp == null)
				return;

			pickedUpItem = detector.ClosestPickUp;

			if (pickedUpItem.PickUpItem(playerGrabTransform)) {
				OnItemPickedUp?.Invoke(pickedUpItem);
			}
		}

		public bool TryReceiveItem(PickUp item) {
			if (pickedUpItem != null) return false;

			if (item.PickUpItem(playerGrabTransform)) {
				pickedUpItem = item;
				OnItemPickedUp?.Invoke(pickedUpItem);
				return true;
			}

			return false;
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

			Rigidbody body = pickedUpItem.GetComponent<Rigidbody>();
			if (body != null)
				body.velocity = playerMovement.Velocity * throwStrengthMultiplier;

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

		public void RoundResetHeldItem() {
			if (pickedUpItem == null) return;

			PickUp item = pickedUpItem;

			DropItem();

			item.HandleNewRoundReset();
		}

		public void OnClosestPickUpChange(PickUp pickUp) { }
	}
}
