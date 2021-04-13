using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactables {

	public class BeerTap : Interactable {

		[SerializeField] private GameObject beerPrefab;
		[SerializeField] private Transform beerSpawnpoint;

		public void Start() {

			onInteraction += HandleOnInteraction;
		}

		private void HandleOnInteraction() {

			if (beerSpawnpoint == null) {
				Debug.LogError("No spawnpoint for beer on beerTap");
				return;
			}

			Debug.Log("Pouring beer...");
			Instantiate(beerPrefab, beerSpawnpoint.position, Quaternion.identity);
		}
	}
}