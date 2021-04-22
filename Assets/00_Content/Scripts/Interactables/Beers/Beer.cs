using UnityEngine;
using Vikings;

namespace Interactables.Beers {
	public class Beer : PickUp, IUseable, IDesirable {

		[SerializeField] private BeerData beerData;

		public DesireType DesireType => beerData.type;

		public void Use(GameObject user) {
			//Drink it? Temporary fighting buff lulz xD

			//Give to customer

			Debug.Log("Using item beer...");
		}

		public void EndUse() { }

	}
}
