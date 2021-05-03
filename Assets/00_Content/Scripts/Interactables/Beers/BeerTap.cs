using System.Collections;
using Rounds;
using Taverns;
using UI;
using UnityEngine;

namespace Interactables.Beers {
	public class BeerTap : Interactable {

		[Header("Settings")]
		[SerializeField] private float pourTime = 1.5f;
		[MinMaxRange(0, 1)]
		[SerializeField] private Vector2 perfectPourMinMax;
		[SerializeField] private int maxBeerAmount = 5;
		[Tooltip("When the amount of beer left in the barrel goes below this amount the fillbar shows continually.")]
		[SerializeField] private int showFillThreshold = 3;

		[Header("GameObjects")]
		[SerializeField] private GameObject beerPrefab;
		[SerializeField] private ProgressBar pourProgressBar;
		[SerializeField] private RectTransform perfectProgressIndicator;
		[SerializeField] private ProgressBar fillProgressBar;
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
			fillProgressBar.UpdateProgress(1);

			// Move perfect pour indicator to fit settings
			Vector2 pourSizeDelta = pourProgressBar.GetComponent<RectTransform>().sizeDelta;

			perfectProgressIndicator.anchoredPosition = new Vector2(perfectPourMinMax.x * pourSizeDelta.x, 0);
			perfectProgressIndicator.sizeDelta = new Vector2(
				(perfectPourMinMax.y - perfectPourMinMax.x) * pourSizeDelta.x,
				perfectProgressIndicator.sizeDelta.y
			);

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += Refill;
		}

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= Refill;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return (Tavern.Instance == null || Tavern.Instance.Money >= beerData.cost) && beerAmount > 0 && !isPouring;
		}

		public override void Interact(GameObject player, PickUp item) {
			if (itemSlot.HasItemInSlot) return;
			isPouring = true;
			StartCoroutine(PouringBeer());
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (!isPouring) return;
			float progress = pouringProgress / pourTime;

			if (progress >= perfectPourMinMax.x && progress <= perfectPourMinMax.y)
				SpawnBeer();
			else
				ResetPouring();
		}

		private IEnumerator PouringBeer() {

			if (Tavern.Instance != null) {
				Tavern.Instance.SpendsMoney(beerData.cost);
			}

			fillProgressBar.Show();
			pourProgressBar.Show();
			perfectProgressIndicator.gameObject.SetActive(true);

			while (!itemSlot.HasItemInSlot && isPouring && pouringProgress <= pourTime) {

				pouringProgress += Time.deltaTime;

				pourProgressBar.UpdateProgress(pouringProgress / pourTime);

				if (pouringProgress > pourTime) {
					SpawnBeer();
					break;
				}

				yield return null;
			}
		}

		private void SpawnBeer() {
			GameObject beer = Instantiate(beerPrefab);
			itemSlot.PlaceItem(beer.GetComponent<PickUp>());

			beerAmount -= 1;

			if (beerAmount > showFillThreshold)
				fillProgressBar.Hide();

			ResetPouring();
		}

		private void ResetPouring() {
			if (!isPouring) return;

			if (Tavern.Instance != null)
				Tavern.Instance.EarnsMoney(beerData.cost);

			pouringProgress = 0;
			isPouring = false;

			fillProgressBar.UpdateProgress(fillPortion * beerAmount);
			pourProgressBar.Hide();
			perfectProgressIndicator.gameObject.SetActive(false);
		}

		public void Refill() {
			beerAmount = maxBeerAmount;
			fillProgressBar.Hide();
			fillProgressBar.UpdateProgress(1);
		}
	}
}
