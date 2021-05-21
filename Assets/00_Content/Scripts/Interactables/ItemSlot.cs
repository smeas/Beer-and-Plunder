using Interactables.Beers;
using Interactables.Instruments;
using Interactables.Weapons;
using System;
using UnityEngine;

namespace Interactables {
	public class ItemSlot : MonoBehaviour {
		private PickUp itemInSlot;
		public bool HasItemInSlot => itemInSlot != null;

		[SerializeField] private ItemType allowedItemType;
		[SerializeField] private bool shouldInheritRotation = false;
		[SerializeField] private bool shouldInheritPosition = false;

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

			if (!IsCorrectItemType(item)) 
				return false;

			if (item.ItemSlotPivot != null && !shouldInheritPosition)
				item.transform.position = OffsetItemCenter(item);
			else
				item.transform.position = transform.position;
			

			Vector3 forward = item.transform.forward;
			forward.y = 0f;

			if (shouldInheritRotation)
				item.transform.rotation = transform.rotation;
			else if (forward.sqrMagnitude > 0.001f)
				item.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			else
				item.transform.rotation = Quaternion.identity;

			//TODO - Remove and rewrite this hardcoded stuff.
			if (allowedItemType == ItemType.All && item is RepairTool) {
				item.transform.position = item.ItemSlotPivot != null ? transform.position + new Vector3(OffsetItemCenter(item).y + -0.4f, -0.4f, 0) : transform.position;
				item.transform.eulerAngles = new Vector3(90, 0, 90);
			}
			if (allowedItemType == ItemType.All && item is Axe) {
				item.transform.position = item.ItemSlotPivot != null ? transform.position + new Vector3(OffsetItemCenter(item).y + -0.4f, -0.4f, 0) : transform.position;
				item.transform.eulerAngles = new Vector3(0, 0, 90);
			}
			if (allowedItemType == ItemType.All && item is Instrument) {
				item.transform.position = item.ItemSlotPivot != null ? transform.position + new Vector3(OffsetItemCenter(item).y + -0.4f, -0.4f, 0) : transform.position;
				item.transform.eulerAngles = new Vector3(-90, 0, 90);
			}

			item.CurrentItemSlot = this;
			itemInSlot = item;
			Rigidbody body = item.GetComponent<Rigidbody>();
			if (body != null)
				body.isKinematic = true;


			return true;
		}

		private Vector3 OffsetItemCenter(PickUp item) {
			 return transform.position + (item.ItemSlotPivot.position - item.transform.position);
		}

		private bool IsCorrectItemType(PickUp item) {

			if (allowedItemType == ItemType.All)
				return true;
			else if (allowedItemType == ItemType.Weapon && item is Axe) 
				return true;
			else if (allowedItemType == ItemType.Instrument && item is Instrument)
				return true;
			else if (allowedItemType == ItemType.RepairTool && item is RepairTool)
				return true;

			return false;
		}

		public void ReleaseItem() {
			Debug.Assert(HasItemInSlot, "ReleaseItem() called with no item in slot", this);
			itemInSlot.CurrentItemSlot = null;
			itemInSlot = null;
		}
	}
}
