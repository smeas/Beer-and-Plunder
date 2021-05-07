using System;
using System.Collections;
using Audio;
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
		[SerializeField] private ProgressBar pourProgressBar;
		[SerializeField] private RectTransform perfectProgressIndicator;
		[SerializeField] private ProgressBar fillProgressBar;
		[SerializeField] private BeerData beerData;

		private float pouringProgress = 0;
		private bool isPouring = false;
		private int beerAmount;
		private float fillPortion;
		private Tankard fillingTankard;
		private SoundHandle pourSoundHandle;

		public int MaxBeerAmount => maxBeerAmount;
		public bool IsFull => beerAmount == maxBeerAmount;

		public event Action BeerPoured;
		public event Action TapRefilled;

		private void Start() {
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
			if (isPouring || beerAmount <= 0)
				return false;

			if (Tavern.Instance != null && Tavern.Instance.Money < beerData.cost)
				return false;

			if (!(item is Tankard tankard) || tankard.IsFull)
				return false;

			return true;
		}

		public override void Interact(GameObject player, PickUp item) {
			isPouring = true;
			fillingTankard = item as Tankard;
			Debug.Assert(fillingTankard != null);

			StartCoroutine(PouringBeer());
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (!isPouring) return;
			float progress = pouringProgress / pourTime;

			if (progress >= perfectPourMinMax.x && progress <= perfectPourMinMax.y) {
				FillBeer();
			}
			else {
				if (Tavern.Instance != null)
					Tavern.Instance.EarnsMoney(beerData.cost);

				ResetPouring();
			}
		}

		private IEnumerator PouringBeer() {
			pourSoundHandle = AudioManager.PlayEffectSafe(SoundEffect.PourBeer);

			if (Tavern.Instance != null) {
				Tavern.Instance.SpendsMoney(beerData.cost);
			}

			fillProgressBar.Show();
			pourProgressBar.Show();
			perfectProgressIndicator.gameObject.SetActive(true);

			while (isPouring && pouringProgress <= pourTime) {

				pouringProgress += Time.deltaTime;

				pourProgressBar.UpdateProgress(pouringProgress / pourTime);

				if (pouringProgress > pourTime) {
					FillBeer();
					break;
				}

				yield return null;
			}

			pourSoundHandle?.FadeOutAndStop(0.2f);
		}

		private void FillBeer() {
			fillingTankard.IsFull = true;
			BeerPoured?.Invoke();

			beerAmount -= 1;

			if (beerAmount > showFillThreshold)
				fillProgressBar.Hide();

			ResetPouring();
		}

		private void ResetPouring() {
			if (!isPouring) return;

			pouringProgress = 0;
			isPouring = false;
			fillingTankard = null;

			fillProgressBar.UpdateProgress(fillPortion * beerAmount);
			pourProgressBar.Hide();
			perfectProgressIndicator.gameObject.SetActive(false);

			pourSoundHandle?.FadeOutAndStop(0.2f);
		}

		public void Refill() {
			beerAmount = maxBeerAmount;
			fillProgressBar.Hide();
			fillProgressBar.UpdateProgress(1);

			TapRefilled?.Invoke();
		}
	}
}
