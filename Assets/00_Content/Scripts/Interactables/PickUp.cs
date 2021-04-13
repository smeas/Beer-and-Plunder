using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
	public class PickUp : MonoBehaviour
	{
		public void OnCollisionStay2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				//PlayerController player = collission.gameObject.GetComponent<PlayerController>();
				//if (player.isPickingUp)
					PickUpInteractable();
			}
		}

		public void PickUpInteractable()
		{
			//player.InventorySlot = this.type;
			Destroy(gameObject);
		}
	}
}
