using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactables;
using UnityEngine;

namespace Player {
	public class InteractionDetector : MonoBehaviour {
		[SerializeField] private GameObject pickUpHighlightPrefab;
		[SerializeField] private GameObject interactableHighlightPrefab;
		[SerializeField] private LayerMask pickUpLayer;
		[SerializeField] private LayerMask interactableLayer;

		private PlayerPickUp playerPickUp;
		private PlayerInteract playerInteract;
		private MonoBehaviour closestObject;
		private GameObject pickUpHighlight;
		private GameObject interactableHighlight;

		private List<PickUp> pickUpsInRange = new List<PickUp>();
		private List<Interactable> interactablesInRange = new List<Interactable>();

		public PickUp ClosestPickUp { get; private set; }
		public Interactable ClosestInteractable { get; private set; }

		private void Start() {
			playerPickUp = GetComponent<PlayerPickUp>();
			playerInteract = GetComponent<PlayerInteract>();

			pickUpHighlight = Instantiate(pickUpHighlightPrefab);
			interactableHighlight = Instantiate(interactableHighlightPrefab);
			pickUpHighlight.SetActive(false);
			interactableHighlight.SetActive(false);
		}

		private void FixedUpdate() {
			UpdateClosestPickup();
			UpdateClosestInteractable();
			UpdateClosestObject();
		}

		private void OnTriggerEnter(Collider other) {
			if (pickUpLayer.ContainsLayer(other.gameObject.layer)) {
				PickUp pickUp = other.GetComponentInParent<PickUp>();
				pickUpsInRange.Add(pickUp);
				pickUp.OnPickedUp += HandleOnPickedUp;
			}
			else if (interactableLayer.ContainsLayer(other.gameObject.layer)) {
				interactablesInRange.Add(other.GetComponentInParent<Interactable>());
			}
		}

		private void OnTriggerExit(Collider other) {
			if (pickUpLayer.ContainsLayer(other.gameObject.layer)) {
				PickUp pickUp = other.GetComponentInParent<PickUp>();
				pickUpsInRange.Remove(pickUp);
				pickUp.OnPickedUp -= HandleOnPickedUp;
			}
			else if (interactableLayer.ContainsLayer(other.gameObject.layer)) {
				interactablesInRange.Remove(other.GetComponentInParent<Interactable>());
			}
		}

		private void HandleOnPickedUp(PickUp item) => pickUpsInRange.Remove(item);

		private void UpdateClosestPickup() {
			PickUp newClosestPickUp = null;
			if (pickUpsInRange.Count > 0)
				newClosestPickUp = pickUpsInRange
					.Where(pickUp => playerPickUp.CanPickUp(pickUp))
					.OrderBy(pickUp => (pickUp.transform.position - transform.position).sqrMagnitude)
					.FirstOrDefault();

			if (newClosestPickUp != ClosestPickUp) {
				ClosestPickUp = newClosestPickUp;
				playerPickUp.OnClosestPickUpChange(ClosestPickUp);
			}

			if (ClosestPickUp != null)
				HighlightPickUp(ClosestPickUp);
			else
				ClearPickUpHighlight();
		}

		private void UpdateClosestInteractable() {
			Interactable newClosestInteractable = null;
			if (interactablesInRange.Count > 0)
				newClosestInteractable = interactablesInRange
					.Where(interactable => playerInteract.CanInteract(interactable))
					.OrderBy(interactable => (interactable.transform.position - transform.position).sqrMagnitude)
					.FirstOrDefault();

			if (newClosestInteractable != ClosestInteractable) {
				ClosestInteractable = newClosestInteractable;
				playerInteract.OnClosestInteractableChange(ClosestInteractable);
			}

			if (ClosestInteractable != null)
				HighlightInteractable(ClosestInteractable);
			else
				ClearInteractableHighlight();
		}

		private void UpdateClosestObject() {
			ClearInteractableHighlight();
			ClearPickUpHighlight();

			bool havePickup = ClosestPickUp != null;
			bool haveInteractable = ClosestInteractable != null;

			if (!havePickup || !haveInteractable) {
				if (haveInteractable) {
					HighlightInteractable(ClosestInteractable);
				}
				else if (havePickup) {
					HighlightPickUp(ClosestPickUp);
				}
				return;
			}

			if ((transform.position - ClosestInteractable.transform.position).sqrMagnitude <
			    (transform.position - ClosestPickUp.transform.position).sqrMagnitude) {
				HighlightInteractable(ClosestInteractable);
			}
			else {
				HighlightPickUp(ClosestPickUp);
			}
		}

		private void HighlightPickUp(PickUp pickUp) {
			if (pickUpHighlight == null)
				pickUpHighlight = Instantiate(pickUpHighlightPrefab);

			pickUpHighlight.transform.position = pickUp.transform.position + Vector3.up * 2;
			pickUpHighlight.SetActive(true);
			closestObject = pickUp;
		}

		private void HighlightInteractable(Interactable interactable) {
			if (interactableHighlight == null)
				interactableHighlight = Instantiate(interactableHighlightPrefab);

			interactableHighlight.transform.position = interactable.transform.position + Vector3.up * 2;
			interactableHighlight.SetActive(true);
			closestObject = interactable;
		}

		private void ClearPickUpHighlight() {
			if (pickUpHighlight == null)
				pickUpHighlight = Instantiate(pickUpHighlightPrefab);

			pickUpHighlight.SetActive(false);
		}

		private void ClearInteractableHighlight() {
			if (interactableHighlight == null)
				interactableHighlight = Instantiate(interactableHighlightPrefab);

			interactableHighlight.SetActive(false);
		}

		// Run from unity event
		public void HandleStartInput() {
			if (closestObject is PickUp && playerPickUp.PickedUpItem == null)
				playerPickUp.PickUpClosestItem();
			else if (playerPickUp.PickedUpItem is IUseable)
				playerPickUp.UseItem();
			else
				playerInteract.Interact();
		}

		// Run from unity event
		public void HandleEndInput() {
			if (playerPickUp.PickedUpItem is IUseable)
				playerPickUp.EndUseItem();
			else
				playerInteract.EndInteract();
		}
	}
}
