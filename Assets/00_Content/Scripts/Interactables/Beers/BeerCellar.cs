using System;
using UnityEngine;

namespace Interactables.Beers {
	public class BeerCellar : MonoBehaviour {
		[SerializeField] private BeerBarrel beerBarrelPrefab;

		private BeerBarrel beerBarrel;

		public event Action<BeerBarrel> beerBarrelSpawn;

		private void Update() {
			if (beerBarrel == null) {
				beerBarrel = Instantiate(beerBarrelPrefab);
				beerBarrel.transform.position = this.transform.position;
				beerBarrelSpawn?.Invoke(beerBarrel);
			}
		}
	}
}
