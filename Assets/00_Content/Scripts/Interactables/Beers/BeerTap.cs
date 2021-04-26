using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Beers {

	public class BeerTap : Interactable {

		[Header("Settings")]
		[Range(1, 100f)]
		[SerializeField] private float pourTimeMultiplier = 10f;

		[Header("GameObjects")]
		[SerializeField] private GameObject beerPrefab;
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

			if (!itemSlot.HasItemInSlot)
				StartCoroutine(PourBeer());
		}

		public override void CancelInteraction() {
			isHolding = false;
		}

		private IEnumerator PourBeer() {

			while (!itemSlot.HasItemInSlot && isHolding && pouringProgress <= 100) {

				pouringProgress += pourTimeMultiplier * Time.deltaTime;

				if(!progressBar.activeInHierarchy)
					progressBar.SetActive(true);

				progressBarImage.fillAmount = pouringProgress * 0.01f;

				if(pouringProgress > 100) {
					GameObject beer = Instantiate(beerPrefab);
					itemSlot.PutItem(beer.GetComponent<PickUp>());

					pouringProgress = 0;
					progressBar.SetActive(false);
					break;
				}

				yield return null;
			}
		}
	}
}
