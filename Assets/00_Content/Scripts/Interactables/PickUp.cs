using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {

	public class PickUp : MonoBehaviour, IPickUp {

		[SerializeField]
		Transform itemGrabTransform;

		public void DropItem() {

			if (itemGrabTransform == null) {
				transform.SetParent(null);
				return;
			}

			itemGrabTransform.SetParent(null);
		}

		public void PickUpItem(Transform playerGrabTransform) {

			//TODO - Move to players hands

			if (itemGrabTransform == null) {
				Debug.Log("No grabPoint set for item, using default position");
				transform.SetParent(playerGrabTransform);
				return;
			}

			itemGrabTransform.SetParent(playerGrabTransform);
		}
	}
}
