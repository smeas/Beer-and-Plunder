using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables {

	public interface IPickUp {

		void PickUpItem(Transform grabTransform);
		void DropItem();
	}
}
