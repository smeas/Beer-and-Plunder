using System;
using System.Linq;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour {
		[SerializeField] private Transform itemGrabTransform;
		[SerializeField] private LayerMask itemSlotLayer;
		[SerializeField] private Collider objectCollider;

		private MeshFilter meshFilter;

		public ItemSlot CurrentItemSlot { get; set; }

		public event Action<PickUp> PickedUp;

		public void Start() {
			meshFilter = GetComponentInChildren<MeshFilter>();
		}

		private void OnDestroy() {
			if (CurrentItemSlot != null) {
				CurrentItemSlot.TakeItem();
				CurrentItemSlot = null;
			}
		}

		//Drop item on floor or snap to slot if close
		public void DropItem() {
			transform.SetParent(null);
			Collider[] collisions =
				Physics.OverlapBox(meshFilter.transform.TransformPoint(meshFilter.mesh.bounds.center),
				                   meshFilter.mesh.bounds.extents, Quaternion.identity, itemSlotLayer);

			if (collisions.Length > 0) {
				ItemSlot closestFreeSlot = collisions
					.Select(col => col.GetComponent<ItemSlot>())
					.Where(slot => !slot.HasItemInSlot)
					.OrderBy(slot => (slot.transform.position - transform.position).sqrMagnitude).FirstOrDefault();

				if (closestFreeSlot != null)
					closestFreeSlot.PutItem(this);
			}

			objectCollider.enabled = true;
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.rotation = Quaternion.identity;
			transform.SetParent(playerGrabTransform);

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;

			if (CurrentItemSlot != null) {
				CurrentItemSlot.TakeItem();
				CurrentItemSlot = null;
			}

			objectCollider.enabled = false;
			PickedUp?.Invoke(this);
		}
	}
}
