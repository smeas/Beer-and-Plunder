using System;
using System.Collections;
using Audio;
using Player;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactables.Beers {
	public class BeerTap : Interactable {

		[Header("Settings")]
		[SerializeField] private float pourTime = 1.5f;
		[MinMaxRange(0, 1)]
		[SerializeField] private Vector2 perfectPourMinMax;
		[Range(0, 1)]
		[SerializeField] private float perfectPourSize;
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
		private RectTransform pourRectTransform;
		private Vector2 perfectPourArea;
		private PlayerMovement pouringPlayer;

		public int MaxBeerAmount => maxBeerAmount;
		public bool IsFull => beerAmount == maxBeerAmount;

		public event Action BeerPoured;
		public event Action TapRefilled;

		private void Start() {
			beerAmount = MaxBeerAmount;
			fillPortion = 1f / MaxBeerAmount;
			fillProgressBar.UpdateProgress(1);

			pourRectTransform = pourProgressBar.GetComponent<RectTransform>();
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			if (isPouring || beerAmount <= 0)
				return false;

			if (!(item is Tankard tankard) || tankard.IsFull)
				return false;

			return true;
		}

		public override void Interact(GameObject player, PickUp item) {
			isPouring = true;
			fillingTankard = item as Tankard;
			Debug.Assert(fillingTankard != null);

			MovePerfectPourArea();
			StartCoroutine(PouringBeer());

			pouringPlayer = player.GetComponent<PlayerMovement>();
			pouringPlayer.BlockMovement();
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (!isPouring) return;
			float progress = pouringProgress / pourTime;

			if (progress >= perfectPourArea.x && progress <= perfectPourArea.y)
				FillBeer();
			else
				ResetPouring();
		}

		private void MovePerfectPourArea() {
			Vector2 pourSizeDelta = pourRectTransform.sizeDelta;
			perfectPourArea.x = Random.Range(perfectPourMinMax.x, perfectPourMinMax.y);
			perfectPourArea.y = perfectPourArea.x + perfectPourSize;

			perfectProgressIndicator.anchoredPosition = new Vector2(perfectPourArea.x * pourSizeDelta.x, 0);
			perfectProgressIndicator.sizeDelta = new Vector2(
				(perfectPourArea.y - perfectPourArea.x) * pourSizeDelta.x,
				perfectProgressIndicator.sizeDelta.y
			);
		}

		private IEnumerator PouringBeer() {
			pourSoundHandle = AudioManager.PlayEffectSafe(SoundEffect.PourBeer);

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

			ResetPouring();
		}

		private void ResetPouring() {
			if (!isPouring) return;

			pouringProgress = 0;
			isPouring = false;
			fillingTankard = null;

			fillProgressBar.UpdateProgress(fillPortion * beerAmount);
			if (beerAmount > showFillThreshold)
				fillProgressBar.Hide();

			pourProgressBar.Hide();
			perfectProgressIndicator.gameObject.SetActive(false);

			pourSoundHandle?.FadeOutAndStop(0.2f);
			pouringPlayer.UnblockMovement();
			pouringPlayer = null;
		}

		public void Refill() {
			beerAmount = maxBeerAmount;
			fillProgressBar.Hide();
			fillProgressBar.UpdateProgress(1);

			TapRefilled?.Invoke();
		}
	}
}
