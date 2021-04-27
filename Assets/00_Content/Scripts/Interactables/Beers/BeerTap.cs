using System.Collections;
using Taverns;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Beers {
	public class BeerTap : Interactable {

		[Header("Settings")]
		[Range(1, 100f)]
		[SerializeField] private float pourTimeMultiplier = 10f;
		[SerializeField] private int maxBeerAmount = 25;
		[SerializeField] private int showFillThreshold = 3;

		[Header("GameObjects")]
		[SerializeField] private GameObject beerPrefab;
		[SerializeField] private Image progressBarImage;
		[SerializeField] private GameObject progressBar;
		[SerializeField] private Image fillBarImage;
		[SerializeField] private GameObject fillBar;
		[SerializeField] private BeerData beerData;

		private ItemSlot itemSlot;
		private float pouringProgress = 0;
		private bool isHolding = false;
		private int beerAmount;

		public int MaxBeerAmount => maxBeerAmount;

		public int BeerAmount {
			get { return beerAmount; }
			set { beerAmount = Mathf.Clamp(beerAmount, 0, maxBeerAmount); }
		}

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();
			beerAmount = maxBeerAmount;
		}

		public override void Interact(GameObject player, PickUp item) {

			isHolding = true;

			if (itemSlot.HasItemInSlot)
				return;

			StartCoroutine(PourBeer());
		}

		public override void CancelInteraction() {
			isHolding = false;
		}

		private IEnumerator PourBeer() {
			if (Tavern.Instance != null && Tavern.Instance.Money < beerData.cost && beerAmount > 0)
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
					BeerAmount--;
					//Need 2 ask J or J about this line of code later
					if (BeerAmount <= showFillThreshold && !fillBar.activeInHierarchy) {
						fillBar.SetActive(true);
						//TODO: I think I need 2 check/set the maximum of the progressbar someway here. So that it is set to maxBeerAmount?
						fillBarImage.fillAmount = BeerAmount * 0.01f;
					}
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