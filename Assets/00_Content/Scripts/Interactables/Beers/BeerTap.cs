using System.Collections;
using Taverns;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Beers {
	public class BeerTap : Interactable {

		[Header("Settings")]
		[Range(1, 100f)]
		[SerializeField] private float pourTimeMultiplier = 10f;
		[SerializeField] private int maxBeerAmount = 5;
		[Tooltip("When the amount of beer left in the barrel goes below this amount the fillbar shows continually.")]
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
		private bool isPouring = false;
		private int beerAmount;
		private float fillPortion;

		public int MaxBeerAmount => maxBeerAmount;
		public bool IsFull => beerAmount == maxBeerAmount;

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();

			beerAmount = MaxBeerAmount;
			fillPortion = 1f / MaxBeerAmount;
			fillBarImage.fillAmount = 1;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return Tavern.Instance.Money >= beerData.cost && beerAmount > 0 && !isPouring;
		}

		public override void Interact(GameObject player, PickUp item) {
			if (itemSlot.HasItemInSlot) return;
			isPouring = true;
			StartCoroutine(PouringBeer());
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (isPouring && Tavern.Instance != null) Tavern.Instance.EarnsMoney(beerData.cost);

			isPouring = false;
		}

		private IEnumerator PouringBeer() {

			if (Tavern.Instance != null) {
				Tavern.Instance.SpendsMoney(beerData.cost);
			}

			fillBar.SetActive(true);

			while (!itemSlot.HasItemInSlot && isPouring && pouringProgress <= 100) {

				pouringProgress += pourTimeMultiplier * Time.deltaTime;

				if (!progressBar.activeInHierarchy)
					progressBar.SetActive(true);

				progressBarImage.fillAmount = pouringProgress * 0.01f;

				if (pouringProgress > 100) {
					GameObject beer = Instantiate(beerPrefab);
					itemSlot.PlaceItem(beer.GetComponent<PickUp>());

					beerAmount -= 1;
					pouringProgress = 0;

					fillBarImage.fillAmount = fillPortion * beerAmount;
					progressBar.SetActive(false);

					isPouring = false;

					if (beerAmount > showFillThreshold) fillBar.SetActive(false);
					break;
				}

				yield return null;
			}
		}

		public void Refill() {
			beerAmount = maxBeerAmount;
			fillBar.SetActive(false);
			fillBarImage.fillAmount = 1;
		}
	}
}
