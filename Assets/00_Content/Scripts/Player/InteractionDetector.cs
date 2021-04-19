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

			playerPickUp.OnItemPickedUp += HandleOnItemPickedUp;
		}

		private void HandleOnItemPickedUp(PickUp item) {
			pickUpsInRange.Remove(item);
		}

		private void FixedUpdate() {
			UpdateClosestPickup();
			UpdateClosestInteractable();
		}

		private void OnTriggerEnter(Collider other) {
			if (pickUpLayer.ContainsLayer(other.gameObject.layer)) {

				pickUpsInRange.Add(FindComponent<PickUp>(other));
			}
			else if (interactableLayer.ContainsLayer(other.gameObject.layer)) {
				interactablesInRange.Add(FindComponent<Interactable>(other));
			}
		}

		private void OnTriggerExit(Collider other) {
			if (pickUpLayer.ContainsLayer(other.gameObject.layer)) {
				pickUpsInRange.Remove(FindComponent<PickUp>(other));
			}
			else if (interactableLayer.ContainsLayer(other.gameObject.layer)) {
				interactablesInRange.Remove(FindComponent<Interactable>(other));
			}
		}

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

		private void HighlightPickUp(PickUp pickUp) {
			pickUpHighlight.transform.position = pickUp.transform.position + Vector3.up * 2;
			pickUpHighlight.SetActive(true);
		}

		private void HighlightInteractable(Interactable interactable) {
			interactableHighlight.transform.position = interactable.transform.position + Vector3.up * 2;
			interactableHighlight.SetActive(true);
		}

		private void ClearPickUpHighlight() {
			pickUpHighlight.SetActive(false);
		}

		private void ClearInteractableHighlight() {
			interactableHighlight.SetActive(false);
		}

		private static T FindComponent<T>(Collider other) where T : Component {
			if (other.attachedRigidbody != null)
				return other.attachedRigidbody.GetComponent<T>();

			return other.GetComponent<T>();
		}
	}
}