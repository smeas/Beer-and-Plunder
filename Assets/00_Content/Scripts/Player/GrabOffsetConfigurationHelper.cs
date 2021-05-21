using Interactables;
using UnityEngine;

namespace Player {
	public class GrabOffsetConfigurationHelper : MonoBehaviour {
	#if UNITY_EDITOR
		[SerializeField] private bool active;

		private PlayerPickUp playerPickUp;

		private void Awake() {
			active = false;
			playerPickUp = GetComponentInParent<PlayerPickUp>();
		}

		private void Update() {
			if (playerPickUp.PickedUpItem == null) return;
			if (active) {
				PickUp pickUp = playerPickUp.PickedUpItem;
				pickUp.ItemGrabOffset = transform.localPosition;
				pickUp.ItemGrabRotation = transform.localEulerAngles;
				pickUp.UpdateGrabPositionOffset();
			}
			else {
				MatchPickUp();
			}
		}

		private void MatchPickUp() {
			transform.localPosition = playerPickUp.PickedUpItem.ItemGrabOffset;
			transform.localEulerAngles = playerPickUp.PickedUpItem.ItemGrabRotation;
		}
	#endif
	}
}