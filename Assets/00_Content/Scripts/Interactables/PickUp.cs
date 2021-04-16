using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour {

		[SerializeField] private Transform itemGrabTransform;
		[SerializeField] private Collider objectCollider;
		[SerializeField] private LayerMask slotLayer;

		//Drop item on floor or snap to slot if close
		public void DropItem() {

			transform.SetParent(null);
			Collider[] collisions = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);

			if(collisions.Length > 0) {

				Collider[] slots = collisions.Where(x => slotLayer.ContainsLayer(x.gameObject.layer)).ToArray();

				if(slots.Length > 0) {
					Collider closestSlot = slots.OrderBy(slot => (slot.transform.position - transform.position).sqrMagnitude).First();
					transform.position = closestSlot.transform.position;
				}
			}

			transform.rotation = Quaternion.identity;
			objectCollider.gameObject.SetActive(true);
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.SetParent(playerGrabTransform);

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;

			objectCollider.gameObject.SetActive(false);
		}
	}
}
