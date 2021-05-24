using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactables;
using UnityEngine;

namespace Player {
	public class InteractionDetector : MonoBehaviour {
		[SerializeField] private ParticleSystem highlightPrefab;
		[SerializeField] private LayerMask pickUpLayer;
		[SerializeField] private LayerMask interactableLayer;

		private PlayerPickUp playerPickUp;
		private PlayerInteract playerInteract;
		private MonoBehaviour closestObject;
		private ParticleSystem highlight;

		private List<PickUp> pickUpsInRange = new List<PickUp>();
		private List<Interactable> interactablesInRange = new List<Interactable>();

		public PickUp ClosestPickUp { get; private set; }
		public Interactable ClosestInteractable { get; private set; }

		private void Awake() {
			playerPickUp = GetComponent<PlayerPickUp>();
			playerInteract = GetComponent<PlayerInteract>();
		}

		private void Start() {
			SetupHighlight();
		}

		private void FixedUpdate() {
			UpdateClosestPickup();
			UpdateClosestInteractable();
			UpdateClosestObject();
		}

		private void OnTriggerEnter(Collider other) {
			if (pickUpLayer.ContainsLayer(other.gameObject.layer)) {
				PickUp pickUp = other.GetComponentInParent<PickUp>();

				if (pickUp != playerPickUp.PickedUpItem) {
					pickUpsInRange.Add(pickUp);
					pickUp.OnPickedUp += HandleOnPickedUp;
				}
			}
			else if (interactableLayer.ContainsLayer(other.gameObject.layer)) {
				Interactable interactable = other.GetComponentInParent<Interactable>();

				// This is needed as vikings may double enter when seating/unseating because of reparenting. Exit
				// should always work though.
				if (!interactablesInRange.Contains(interactable))
					interactablesInRange.Add(interactable);
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

		private void OnDestroy() {
			if (highlight != null)
				Destroy(highlight);
		}

		private void SetupHighlight() {
			highlight = Instantiate(highlightPrefab);

			ParticleSystem[] childEffects = highlight.GetComponentsInChildren<ParticleSystem>();
			Color startColor = GetComponentInParent<PlayerComponent>().PlayerColor;

			foreach (ParticleSystem childEffect in childEffects) {
				ParticleSystem.MainModule effectMain = childEffect.main;
				startColor.a = effectMain.startColor.color.a;
				effectMain.startColor = startColor;
			}
		}

		private void HandleOnPickedUp(PickUp item, PlayerComponent playerComponent) => pickUpsInRange.Remove(item);

		private void UpdateClosestPickup() {
			PickUp newClosestPickUp = null;

			if (pickUpsInRange.Count > 0) {
				List<PickUp> orderedList = new List<PickUp>();

				for (int i = pickUpsInRange.Count - 1; i >= 0; i--) {

					if (pickUpsInRange[i] == null) {
						pickUpsInRange.RemoveAt(i);
						continue;
					}

					if (playerPickUp.CanPickUp(pickUpsInRange[i]))
						orderedList.Add(pickUpsInRange[i]);
				}

				newClosestPickUp = orderedList.OrderBy((x => (x.transform.position - transform.position).sqrMagnitude)).FirstOrDefault();
			}

			if (newClosestPickUp != ClosestPickUp) {
				ClosestPickUp = newClosestPickUp;
				playerPickUp.OnClosestPickUpChange(ClosestPickUp);
			}
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
		}

		private void UpdateClosestObject() {
			bool havePickup = ClosestPickUp != null;
			bool haveInteractable = ClosestInteractable != null;
			bool haveHighlighted = false;
			closestObject = null;

			if (!havePickup || !haveInteractable) {
				if (haveInteractable) {
					HighlightInteractable(ClosestInteractable);
					haveHighlighted = true;
				}
				else if (havePickup) {
					HighlightPickUp(ClosestPickUp);
					haveHighlighted = true;
				}
			}

			if (!haveHighlighted && havePickup) {
				if ((transform.position - ClosestInteractable.transform.position).sqrMagnitude <
				    (transform.position - ClosestPickUp.transform.position).sqrMagnitude) {
					HighlightInteractable(ClosestInteractable);
				}
				else {
					HighlightPickUp(ClosestPickUp);
				}
			}

			if (closestObject == null && highlight != null && highlight.isPlaying)
				highlight.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}

		private void HighlightPickUp(PickUp pickUp) {
			Bounds objectBounds = pickUp.ObjectCollider.bounds;
			MoveHighlight(objectBounds.center - new Vector3(0, objectBounds.extents.y, 0), pickUp.highlightSize);

			closestObject = pickUp;
		}

		private void HighlightInteractable(Interactable interactable) {
			MoveHighlight(interactable.highlightPivot != null
					? interactable.highlightPivot.position
					: interactable.transform.position, interactable.highlightSize);

			closestObject = interactable;
		}

		private void MoveHighlight(Vector3 position, float size) {
			if (highlight == null)
				SetupHighlight();

			Transform highlightTransform = highlight.transform;
			highlightTransform.position = position;
			highlightTransform.localScale = new Vector3(size, size, size);

			if (!highlight.isPlaying)
				highlight.Play(true);
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
