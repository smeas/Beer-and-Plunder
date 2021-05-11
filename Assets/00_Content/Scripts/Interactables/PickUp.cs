using System;
using System.Linq;
using Player;
using Rounds;
using UnityEngine;
using World;

namespace Interactables {

	public class PickUp : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform itemGrabTransform;
		[SerializeField] private LayerMask itemSlotLayer = 1 << 9;
		[SerializeField] protected Collider objectCollider;

		protected new Rigidbody rigidbody;
		private bool isBeingCarried;

		protected bool IsMultiCarried { get; set; }
		public ItemSlot StartItemSlot { private get; set; }
		public ItemSlot CurrentItemSlot { get; set; }

		public event Action<PickUp, PlayerComponent> OnPickedUp;
		public event Action<PickUp, PlayerComponent> OnDropped;

		protected virtual void Start() {
			rigidbody = GetComponent<Rigidbody>();

			if (StartItemSlot == null)
				StartItemSlot = CurrentItemSlot;

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += Respawn;
		}

		protected virtual void OnDestroy() {
			if (CurrentItemSlot != null) {
				CurrentItemSlot.ReleaseItem();
				CurrentItemSlot = null;
			}

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= Respawn;
		}

		public virtual void SetParent(Transform newParent) {
			transform.SetParent(newParent);
		}

		//Drop item on floor or snap to slot if close
		public void DropItem(Transform playerGrabTransform) {

			if (IsMultiCarried) {
				OnDropped?.Invoke(this, playerGrabTransform.GetComponentInParent<PlayerComponent>());
				return;
			}

			SetParent(null);
			if (rigidbody != null)
				rigidbody.isKinematic = false;

			// Need to enable the collider fist because otherwise it will have zero sized bounds.
			objectCollider.enabled = true;

			TryPutInClosestItemSlot();
			isBeingCarried = false;
			OnDropped?.Invoke(this, playerGrabTransform.GetComponentInParent<PlayerComponent>());
		}

		public bool PickUpItem(Transform playerGrabTransform) {
			if (IsMultiCarried)
				return false;

			transform.rotation = Quaternion.identity;
			SetParent(playerGrabTransform);
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
			isBeingCarried = true;
			OnPickedUp?.Invoke(this, playerGrabTransform.GetComponentInParent<PlayerComponent>());
			return true;
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

				if (closestFreeSlot != null) {
					closestFreeSlot.PlaceItem(this);
					OnPlace();
				}
			}
		}

		public virtual void Respawn() {
			//Unsure if we really want/need this check anymore
			if (isBeingCarried) return;
		}

		protected virtual void OnPlace() {}

	#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			Gizmos.DrawWireCube(objectCollider.bounds.center, objectCollider.bounds.size);
		}
	#endif
	}
}
