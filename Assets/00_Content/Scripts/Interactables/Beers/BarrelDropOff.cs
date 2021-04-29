using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Beers {
	public class BarrelDropOff : Interactable {
		[SerializeField] private Image progressBarImage;
		[SerializeField] private GameObject progressBar;
		[SerializeField] private BeerTap beerTap;
		[Tooltip("Timespan in s it takes to switch beerbarrels.")]
		[SerializeField] private float switchTime = 2f;

		private float switchingProgress = 0f;
		private bool isHolding;

		public override bool CanInteract(GameObject player, PickUp item) {
			return item is BeerBarrel && beerTap.BeerAmount < beerTap.MaxBeerAmount;
		}

		public override void Interact(GameObject player, PickUp barrel) {
			isHolding = true;
			StartCoroutine(SwitchBeerTapBarrel(barrel));
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			isHolding = false;
		}

		private IEnumerator SwitchBeerTapBarrel(PickUp barrel) {

			while (switchingProgress <= switchTime && isHolding) {

				switchingProgress += Time.deltaTime;

				if (!progressBar.activeInHierarchy) progressBar.SetActive(true);

				progressBarImage.fillAmount = switchingProgress / switchTime;

				if (switchingProgress > switchTime) {
					beerTap.BeerAmount = beerTap.MaxBeerAmount;
					Destroy(barrel.gameObject);
					switchingProgress = 0;
					progressBar.SetActive(false);

					beerTap.BeerTapFilledUp();
					break;
				}

				yield return null;
			}
		}
	}
}