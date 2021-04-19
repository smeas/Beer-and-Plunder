using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour {

		[SerializeField] private Transform itemGrabTransform;
		[SerializeField] private LayerMask itemSlotLayer;

		private Collider coll;
		private MeshFilter meshFilter;

		public void Start() {
			coll = GetComponentInChildren<Collider>();
			meshFilter = GetComponentInChildren<MeshFilter>();
		}

		//Drop item on floor or snap to slot if close
		public void DropItem() {

			transform.SetParent(null);
			Collider[] collisions = Physics.OverlapBox(meshFilter.transform.TransformPoint(meshFilter.mesh.bounds.center),
				meshFilter.mesh.bounds.extents, Quaternion.identity);

			if(collisions.Length > 0) {

				Collider[] slots = collisions.Where(x => itemSlotLayer.ContainsLayer(x.gameObject.layer)).ToArray();

				if(slots.Length > 0) {
					Collider closestSlot = slots.OrderBy(slot => (slot.transform.position - transform.position).sqrMagnitude).First();
					var itemSlot = closestSlot.gameObject.GetComponent<ItemSlot>();
					
					if(!itemSlot.HasItemInSlot)
						transform.position = closestSlot.transform.position;
				}
			}

			transform.rotation = Quaternion.identity;
			coll.gameObject.SetActive(true);
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.SetParent(playerGrabTransform);

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;

			coll.gameObject.SetActive(false);
		}
	}
}
