using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour {

		[SerializeField] private Transform itemGrabTransform;

		public void DropItem() {
			transform.SetParent(null);
			transform.rotation = Quaternion.identity;
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.SetParent(playerGrabTransform);

			Vector3 offset = Vector3.zero;
			if (itemGrabTransform != null)
				offset = transform.position - itemGrabTransform.position;

			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;
		}
	}
}
