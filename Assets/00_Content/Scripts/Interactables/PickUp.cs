using System;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour {

		[SerializeField] private Transform itemGrabTransform;
		[SerializeField] private LayerMask itemSlotLayer;
		[SerializeField] private Collider objectCollider;

		private MeshFilter meshFilter;

		protected Collider ObjectCollider => objectCollider;
		public event Action<PickUp> OnPickedUp;
		public event Action<PickUp> OnDropped;


		public virtual void Start() {
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

			objectCollider.enabled = true;
			OnDropped?.Invoke(this);
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.rotation = Quaternion.identity;
			transform.SetParent(playerGrabTransform);

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;

			objectCollider.enabled = false;
			OnPickedUp?.Invoke(this);
		}
	}
}
