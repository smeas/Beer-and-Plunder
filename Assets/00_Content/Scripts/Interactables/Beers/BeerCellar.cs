using UnityEngine;

namespace Interactables.Beers {
	public class BeerCellar : Interactable {
		[SerializeField] private BeerBarrel beerBarrelPrefab;
		[SerializeField] private ItemSlot barrelSpawnSlot;

		private BeerBarrel beerBarrel;

		private void Start() {
			barrelSpawnSlot = GetComponentInChildren<ItemSlot>();
		}

		private void Update() {
			if (beerBarrel == null) {
				beerBarrel = Instantiate(beerBarrelPrefab);
				barrelSpawnSlot.PlaceItem(beerBarrel);
			}
		}
	}
}
