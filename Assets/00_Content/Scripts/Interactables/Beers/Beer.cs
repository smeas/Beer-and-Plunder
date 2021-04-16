using UnityEngine;

namespace Interactables.Beers {
	public class Beer : PickUp, IUseable {

		[SerializeField] private BeerData beerData;
		
		public void Use() {
			//Drink it? Temporary fighting buff lulz xD

			//Give to customer

			Debug.Log("Using item beer...");
		}
	}
}
