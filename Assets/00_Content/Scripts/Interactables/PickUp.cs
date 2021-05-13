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
		[SerializeField] public Transform ItemSlotPivot;

		protected new Rigidbody rigidbody;
		private bool isBeingCarried;

		private Vector3 startPosition;
		private Quaternion startRotation;

		public bool IsMultiCarried { get; protected set; }
		public ItemSlot StartItemSlot { private get; set; }
		public ItemSlot CurrentItemSlot { get; set; }
		public virtual bool IsHeavy => false;

		public event Action<PickUp, PlayerComponent> OnPickedUp;
		public event Action<PickUp, PlayerComponent> OnDropped;

		protected virtual void Start() {
			rigidbody = GetComponent<Rigidbody>();

			startPosition = transform.position;
			startRotation = transform.rotation;

			if (StartItemSlot == null)
				StartItemSlot = CurrentItemSlot;

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += RoundOverReset;
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

			if (!isBeingCarried)
				MoveToPoint(playerGrabTransform);

			if (CurrentItemSlot != null) {
				CurrentItemSlot.ReleaseItem();
				CurrentItemSlot = null;
			}

			objectCollider.enabled = false;
			isBeingCarried = true;
			OnPickedUp?.Invoke(this, playerGrabTransform.GetComponentInParent<PlayerComponent>());
			return true;
		}

		protected void MoveToPoint(Transform point) {
			transform.rotation = Quaternion.identity;
			SetParent(point);

			if (rigidbody != null)
				rigidbody.isKinematic = true;

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;
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

		public virtual void RoundOverReset() {
			if (isBeingCarried) return;

		}

		public virtual void Respawn() {

			if (isBeingCarried) return;

			transform.SetPositionAndRotation(startPosition, startRotation);

			// Put the item back into its original slot
			if (StartItemSlot != null) {
				if (StartItemSlot.HasItemInSlot)
					StartItemSlot.ReleaseItem();

				StartItemSlot.PlaceItem(this);
			} else if (CurrentItemSlot != null) {
				CurrentItemSlot.ReleaseItem();
			}

			RoundOverReset();
		}

		protected virtual void OnPlace() {}

	#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			if (objectCollider != null)
				Gizmos.DrawWireCube(objectCollider.bounds.center, objectCollider.bounds.size);
		}
	#endif
	}
}
