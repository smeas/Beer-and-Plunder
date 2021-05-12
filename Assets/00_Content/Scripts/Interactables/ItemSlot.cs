using Interactables.Beers;
using UnityEngine;

namespace Interactables {
	public class ItemSlot : MonoBehaviour {
		private PickUp itemInSlot;
		public bool HasItemInSlot => itemInSlot != null;

		private void Start() {
			PickUp currentPickup = GetComponentInChildren<PickUp>();
			if (currentPickup != null) {
				currentPickup.transform.SetParent(null);
				currentPickup.StartItemSlot = this;
				currentPickup.CurrentItemSlot = this;
				PlaceItem(currentPickup);
			}
		}

		public bool PlaceItem(PickUp item) {
			if (HasItemInSlot || item is BeerBarrel)
				return false;

			item.transform.position = transform.position;
			item.CurrentItemSlot = this;
			itemInSlot = item;

			Rigidbody body = item.GetComponent<Rigidbody>();
			if (body != null)
				body.isKinematic = true;

			return true;
		}

		public void ReleaseItem() {
			Debug.Assert(HasItemInSlot, "ReleaseItem() called with no item in slot", this);
			itemInSlot.CurrentItemSlot = null;
			itemInSlot = null;
		}
	}
}
