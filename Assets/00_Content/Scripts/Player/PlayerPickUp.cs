using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Player {

	public class PlayerPickUp : MonoBehaviour {

		[SerializeField] private Transform playerGrabTransform;

		//Should probably be displayed on a canvas instead.
		[SerializeField] private GameObject highlight;

		private List<PickUp> pickUps;
		private PickUp closestPickUp;
		private PickUp pickedUpItem;
		private SphereCollider sphereCollider;

		private void Start() {

			pickUps = new List<PickUp>();
			sphereCollider = GetComponent<SphereCollider>();
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

			pickedUpItem.DropItem();
			pickedUpItem = null;
			sphereCollider.enabled = true;
		}

		public void UseItem() {
			if (pickedUpItem != null && pickedUpItem is IUseable)
				Debug.Log("Using item...");
		}

		private void OnTriggerEnter(Collider other) {

			if (other.gameObject.CompareTag("PickUp") && pickedUpItem == null) {

				pickUps.Add(other.gameObject.GetComponent<PickUp>());
				closestPickUp = pickUps.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude).First();
				HighlightPickUp(closestPickUp);
			}
		}

		private void OnTriggerExit(Collider other) {

			if (other.gameObject.CompareTag("PickUp") && pickedUpItem == null) {

				pickUps.Remove(other.gameObject.GetComponent<PickUp>());

				if (pickUps.Count < 1) {

					HighlightPickUp(null);
					return;
				}

				closestPickUp = pickUps.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude).First();
				HighlightPickUp(closestPickUp);
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
