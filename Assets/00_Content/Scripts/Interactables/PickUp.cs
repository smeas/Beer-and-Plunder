using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour {

		[SerializeField] Transform itemGrabTransform;

		public void DropItem() {
			transform.SetParent(null);
			transform.rotation = Quaternion.identity;
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.SetParent(playerGrabTransform);
			Vector3 offset = transform.position - itemGrabTransform.position;
			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;
		}
	}
}
