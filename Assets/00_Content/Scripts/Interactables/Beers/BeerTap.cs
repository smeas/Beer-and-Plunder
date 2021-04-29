using System;
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

		public int BeerAmount {
			get { return beerAmount; }
			set { beerAmount = Mathf.Clamp(value, 0, MaxBeerAmount); }
		}

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();

			beerAmount = MaxBeerAmount;
			fillPortion = 1f / MaxBeerAmount;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return Tavern.Instance.Money >= beerData.cost && BeerAmount >= 1 && !isPouring;
		}

		public override void Interact(GameObject player, PickUp item) {
			isPouring = true;
			if (itemSlot.HasItemInSlot) return;

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

			while (!itemSlot.HasItemInSlot && isPouring && pouringProgress <= 100) {

				fillBarImage.fillAmount = fillPortion * beerAmount;
				fillBar.SetActive(true);

				pouringProgress += pourTimeMultiplier * Time.deltaTime;

				if (!progressBar.activeInHierarchy)
					progressBar.SetActive(true);

				progressBarImage.fillAmount = pouringProgress * 0.01f;

				if (pouringProgress > 100) {
					GameObject beer = Instantiate(beerPrefab);
					itemSlot.PlaceItem(beer.GetComponent<PickUp>());
					BeerAmount -= 1;
					pouringProgress = 0;
					progressBar.SetActive(false);
					isPouring = false;

					if (BeerAmount > showFillThreshold) fillBar.SetActive(false);
					break;
				}

				yield return null;
			}
		}
		/// <summary>
		/// Function calculates the fillBars value and deactivates fillBar.
		/// </summary>
		public void BeerTapFilledUp() {
			fillBarImage.fillAmount = fillPortion * beerAmount;
			fillBar.SetActive(false);
		}
	}
}