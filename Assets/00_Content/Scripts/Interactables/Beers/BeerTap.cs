using System.Collections;
using Taverns;
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
		[SerializeField] private BeerData beerData;

		private ItemSlot itemSlot;
		private float pouringProgress = 0;
		private bool isHolding = false;

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();
		}

		public override void Interact(GameObject player, PickUp item) {

			isHolding = true;

			if (itemSlot.HasItemInSlot)
				return;

			StartCoroutine(PourBeer());
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			isHolding = false;
		}

		private IEnumerator PourBeer() {
			if (Tavern.Instance != null && Tavern.Instance.Money < beerData.cost)
				yield break;

			while (!itemSlot.HasItemInSlot && isHolding && pouringProgress <= 100) {
				if (Tavern.Instance != null && Tavern.Instance.Money < beerData.cost)
					break;

				pouringProgress += pourTimeMultiplier * Time.deltaTime;

				if (!progressBar.activeInHierarchy)
					progressBar.SetActive(true);

				progressBarImage.fillAmount = pouringProgress * 0.01f;

				if (pouringProgress > 100) {
					GameObject beer = Instantiate(beerPrefab);
					itemSlot.PlaceItem(beer.GetComponent<PickUp>());

					if (Tavern.Instance != null)
						Tavern.Instance.SpendsMoney(beerData.cost);

					pouringProgress = 0;
					progressBar.SetActive(false);
					break;
				}

				yield return null;
			}
		}
	}
}
