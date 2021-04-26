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
				PutItem(currentPickup);
			}
		}

		public bool PutItem(PickUp item) {
			if (HasItemInSlot)
				return false;

			item.transform.position = transform.position;
			item.CurrentItemSlot = this;
			HasItemInSlot = true;

			item.GetComponent<Rigidbody>().isKinematic = true;

			return true;
		}

		public void TakeItem() {
			Debug.Assert(HasItemInSlot, "TakeItem() called with no item in slot", this);
			HasItemInSlot = false;
		}
	}
}
