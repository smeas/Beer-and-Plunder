using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Beers {
	public class BarrelDropOff : Interactable {
		[SerializeField] private Image progressBarImage;
		[SerializeField] private GameObject progressBar;
		[SerializeField] private BeerTap beerTap;

		public override bool CanInteract(GameObject player, PickUp item) {
			return item is BeerBarrel && beerTap.BeerAmount < 1;
		}

		public override void Interact(GameObject player, PickUp item) {
			base.Interact(player, item);
		}

		//Look at cancel interact override

		//Okay so, code here will need to set the BeerTap to its maximum, but also turn of the fillBar,
		//seeing as it will be above the threshold value, otherwise I will need to move the set active/inactive on the fillBar in BeerTap to LateUpdate().

		//Also, some way here to destroy barrel, and also some way here to tell the BeerCellar to spawn a new Barrel(when this one is destroyed).
	}
}

