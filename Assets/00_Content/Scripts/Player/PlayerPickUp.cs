using System.Collections.Generic;
using System.Linq;
using Interactables;
using UnityEngine;

namespace Player {

	public class PlayerPickUp : MonoBehaviour {

		[SerializeField] private Transform playerGrabTransform;
		[SerializeField] private GameObject playerRoot;

		//Should probably be displayed on a canvas instead.
		[SerializeField] private GameObject highlightPrefab;
		[SerializeField] private LayerMask pickUpLayer;

		private List<PickUp> pickUps;
		private PickUp closestPickUp;
		private PickUp pickedUpItem;
		private SphereCollider sphereCollider;
		private GameObject highlight;
		private bool isUsingItem;

		private void Start() {

			pickUps = new List<PickUp>();
			sphereCollider = GetComponent<SphereCollider>();

			highlight = Instantiate(highlightPrefab);
			highlight.SetActive(false);
		}

		private void FixedUpdate() {

			if (pickUps.Count <= 0) {
				return;
			}

			closestPickUp = pickUps.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude).First();
			HighlightPickUp(closestPickUp);
		}

		public void PickUpClosestItem() {

			HighlightPickUp(null);

			if (pickUps.Count < 1 || closestPickUp == null)
				return;

			closestPickUp.PickUpItem(playerGrabTransform);
			pickedUpItem = closestPickUp;
			pickUps.Clear();
			sphereCollider.enabled = false;
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
			sphereCollider.enabled = true;
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

		private void OnTriggerEnter(Collider other) {

			if ((pickUpLayer & (1 << other.gameObject.layer)) != 0 && pickedUpItem == null) {

				pickUps.Add(other.attachedRigidbody.GetComponent<PickUp>());
			}
		}

		private void OnTriggerExit(Collider other) {

			if ((pickUpLayer & (1 << other.gameObject.layer)) != 0 && pickedUpItem == null) {

				pickUps.Remove(other.attachedRigidbody.GetComponent<PickUp>());

				if (pickUps.Count <= 0) {
					HighlightPickUp(null);
				}
			}
		}

		private void HighlightPickUp(PickUp pickUp) {

			if(pickUp == null) {
				highlight.SetActive(false);
				return;
			}

			highlight.transform.position = pickUp.transform.position + Vector3.up;
			highlight.SetActive(true);
			//TODO - Render pickUp icon/button on item in worldspace canvas.
		}
	}
}
