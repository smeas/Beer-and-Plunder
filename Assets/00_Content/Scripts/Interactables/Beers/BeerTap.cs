using Extensions;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Interactables.Beers {

	public class BeerTap : Interactable {

		[Header("Settings")]
		[Range(1, 100f)]
		[SerializeField] private float pourTimeMultiplier = 10f;

		[Header("GameObjects")]
		[SerializeField] private GameObject beerPrefab;
		[SerializeField] private Transform beerSpawnpoint;
		[SerializeField] private Image progressBarImage;
		[SerializeField] private GameObject progressBar;

		private ItemSlot itemSlot;
		private float pouringProgress = 0;
		private bool isHolding = false;

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();
		}

		public override void Interact(GameObject player, PickUp item) {

			isHolding = true;

			if (beerSpawnpoint == null) {
				Debug.LogError("No spawnpoint for beer on beerTap");
				return;
			}

			if (itemSlot.HasItemInSlot) {
				Debug.Log("Can´t pour, has full beer in slot");
				return;
			}

			StartCoroutine(PourBeer());
		}

		public override void CancelInteraction() {
			Debug.Log("Cancel interaction!");
			isHolding = false;
		}

		private IEnumerator PourBeer() {

			while (!itemSlot.HasItemInSlot && isHolding && pouringProgress <= 100) {
				
				pouringProgress += pourTimeMultiplier * Time.deltaTime; 

				if(!progressBar.activeInHierarchy)
					progressBar.SetActive(true);

				progressBarImage.fillAmount = pouringProgress * 0.01f;

				if(pouringProgress > 100) {
					Instantiate(beerPrefab, beerSpawnpoint.position, Quaternion.identity);
					pouringProgress = 0;
					progressBar.SetActive(false);
					break;
				}

				yield return null;
			}
		}
	}
}