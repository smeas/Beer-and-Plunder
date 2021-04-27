using System;
using System.Linq;
using UnityEngine;
using World;

namespace Interactables {

	public class PickUp : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform itemGrabTransform;
		[SerializeField] private LayerMask itemSlotLayer = 1 << 9;
		[SerializeField] private Collider objectCollider;

		private new Rigidbody rigidbody;
		private Vector3 startPosition;
		private Quaternion startRotation;

		public ItemSlot StartItemSlot { private get; set; }
		public ItemSlot CurrentItemSlot { get; set; }

		public event Action<PickUp> PickedUp;

		public void Start() {
			rigidbody = GetComponent<Rigidbody>();

			startPosition = transform.position;
			startRotation = transform.rotation;
		}

		private void OnDestroy() {
			if (CurrentItemSlot != null) {
				CurrentItemSlot.ReleaseItem();
				CurrentItemSlot = null;
			}
		}

		//Drop item on floor or snap to slot if close
		public void DropItem() {
			transform.SetParent(null);
			if (rigidbody != null)
				rigidbody.isKinematic = false;

			// Need to enable the collider fist because otherwise it will have zero sized bounds.
			objectCollider.enabled = true;

			TryPutInClosestItemSlot();
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.rotation = Quaternion.identity;
			transform.SetParent(playerGrabTransform);
			if (rigidbody != null)
				rigidbody.isKinematic = true;

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;

			if (CurrentItemSlot != null) {
				CurrentItemSlot.ReleaseItem();
				CurrentItemSlot = null;
			}

			objectCollider.enabled = false;
			PickedUp?.Invoke(this);
		}

		private void TryPutInClosestItemSlot() {
			Bounds bounds = objectCollider.bounds;
			Collider[] collisions =
				Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, itemSlotLayer);

			if (collisions.Length > 0) {
				ItemSlot closestFreeSlot = collisions
					.Select(col => col.GetComponent<ItemSlot>())
					.Where(slot => !slot.HasItemInSlot)
					.OrderBy(slot => (slot.transform.position - transform.position).sqrMagnitude).FirstOrDefault();

				if (closestFreeSlot != null)
					closestFreeSlot.PlaceItem(this);
			}
		}

		public void Respawn() {
			transform.SetPositionAndRotation(startPosition, startRotation);

			// Put the item back into its original slot if possible
			if (StartItemSlot != null && !StartItemSlot.HasItemInSlot)
				StartItemSlot.PlaceItem(this);
		}

		private void OnDrawGizmosSelected() {
			Gizmos.DrawWireCube(objectCollider.bounds.center, objectCollider.bounds.size);
		}
	}
}
