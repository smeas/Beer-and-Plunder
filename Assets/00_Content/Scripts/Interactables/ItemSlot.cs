using UnityEngine;

namespace Interactables {
	public class ItemSlot : MonoBehaviour {
		public bool HasItemInSlot { get; private set; }

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
			if (HasItemInSlot)
				return false;

			item.transform.position = transform.position;
			item.CurrentItemSlot = this;
			HasItemInSlot = true;

			Rigidbody body = item.GetComponent<Rigidbody>();
			if (body != null)
				body.isKinematic = true;

			return true;
		}

		public void ReleaseItem() {
			Debug.Assert(HasItemInSlot, "ReleaseItem() called with no item in slot", this);
			HasItemInSlot = false;
		}
	}
}
