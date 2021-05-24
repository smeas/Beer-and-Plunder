using DG.Tweening;
using System;
using System.Linq;
using Player;
using Rounds;
using UnityEngine;
using Vikings;
using World;

namespace Interactables {

	public class PickUp : MonoBehaviour, IRespawnable {
		[SerializeField] private Vector3 itemGrabOffset;
		[SerializeField] private Vector3 itemGrabRotation;
		[SerializeField] private LayerMask itemSlotLayer = 1 << 9;
		[SerializeField] protected Collider objectCollider;
		[SerializeField] public Transform ItemSlotPivot;
		[SerializeField] public float highlightSize = 0.5f;

		protected new Rigidbody rigidbody;
		protected bool isBeingCarried;

		private Vector3 startPosition;
		private Quaternion startRotation;

		[SerializeField] protected float shrinkTime = 1.0f;

		public bool IsMultiCarried { get; protected set; }
		public ItemSlot StartItemSlot { private get; set; }
		public ItemSlot CurrentItemSlot { get; set; }
		public virtual bool IsHeavy => false;
		public Collider ObjectCollider => objectCollider;

		public Vector3 ItemGrabOffset {
			get => itemGrabOffset;
			set => itemGrabOffset = value;
		}

		public Vector3 ItemGrabRotation {
			get => itemGrabRotation;
			set => itemGrabRotation = value;
		}

		public event Action<PickUp, PlayerComponent> OnPickedUp;
		public event Action<PickUp, PlayerComponent> OnDropped;

		protected virtual void Awake() {
			rigidbody = GetComponent<Rigidbody>();
		}

		protected virtual void Start() {
			startPosition = transform.position;
			startRotation = transform.rotation;

			if (StartItemSlot == null)
				StartItemSlot = CurrentItemSlot;

			if (RoundController.Instance != null) {
				RoundController.Instance.OnNewRoundStart += HandleNewRoundReset;
			}
		}

		protected virtual void OnDestroy() {
			if (CurrentItemSlot != null) {
				CurrentItemSlot.ReleaseItem();
				CurrentItemSlot = null;
			}

			if (RoundController.Instance != null) {
				RoundController.Instance.OnRoundOver -= Respawn;
				RoundController.Instance.OnNewRoundStart -= HandleNewRoundReset;
			}
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

		public void VikingPickUpItem(Viking viking) {
			MoveToPoint(viking.handTransform);

			objectCollider.enabled = false;
			isBeingCarried = true;
		}

		public void VikingDropItem() {
			SetParent(null);
			if (rigidbody != null)
				rigidbody.isKinematic = false;

			objectCollider.enabled = true;
			isBeingCarried = false;
		}

		protected void MoveToPoint(Transform point) {
			SetParent(point);

			if (rigidbody != null)
				rigidbody.isKinematic = true;

			UpdateGrabPositionOffset();
		}

		public void UpdateGrabPositionOffset() {
			transform.localPosition = itemGrabOffset;
			transform.localEulerAngles = itemGrabRotation;
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

		public virtual void HandleNewRoundReset() {
			if (isBeingCarried) return;

		}
		/// <summary>
		/// Used in override on HandleNewRoundReset to ensure things shrink away and disappears
		/// </summary>
		protected void ShrinkAway() {
			transform.DOScale(Vector3.zero, shrinkTime).OnComplete(() => {
				Destroy(gameObject);
			});
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
		}

		protected virtual void OnPlace() { }

#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			if (objectCollider != null)
				Gizmos.DrawWireCube(objectCollider.bounds.center, objectCollider.bounds.size);
		}

		private void OnValidate() {
			if (Application.isPlaying && isBeingCarried) {
				UpdateGrabPositionOffset();
			}
		}
	#endif
	}
}
