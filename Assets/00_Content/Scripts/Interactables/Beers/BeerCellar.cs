using UnityEngine;

namespace Interactables.Beers {
	public class BeerCellar : MonoBehaviour {
		[SerializeField] private BeerBarrel beerBarrelPrefab;

		private BeerBarrel beerBarrel;

		private void Update() {
			if (beerBarrel == null) {
				beerBarrel = Instantiate(beerBarrelPrefab);
				beerBarrel.transform.position = this.transform.position;
			}
		}
	}
}
