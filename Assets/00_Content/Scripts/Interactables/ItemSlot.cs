using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {
	public class ItemSlot : MonoBehaviour {

		[SerializeField] private LayerMask pickUpLayer;

		private bool hasItemInSlot = false;

		public bool HasItemInSlot => hasItemInSlot;

		private void FixedUpdate() {

			var collisions = Physics.OverlapBox(transform.position, transform.localScale / 2 * 0.9f, Quaternion.identity);
			var pickUps = collisions.Where(x => pickUpLayer.ContainsLayer(x.gameObject.layer)).ToList();

			if (pickUps.Count > 0)
				hasItemInSlot = true;
			else
				hasItemInSlot = false;
		}
	}
}
