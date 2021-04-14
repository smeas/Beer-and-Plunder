using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour, IPickUp {

		[SerializeField] Transform itemGrabTransform;

		public void DropItem() {
			transform.SetParent(null);
			transform.rotation = Quaternion.identity;
		}

		public void PickUpItem(Transform playerGrabTransform) {
			transform.SetParent(playerGrabTransform);
			var offset = transform.position - itemGrabTransform.position;
			transform.localPosition = offset;
			transform.localRotation = Quaternion.identity;
		}
	}
}
