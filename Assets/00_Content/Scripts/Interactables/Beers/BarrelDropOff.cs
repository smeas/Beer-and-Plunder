using System.Collections;
using Player;
using UI;
using UnityEngine;

namespace Interactables.Beers {
	public class BarrelDropOff : Interactable {
		[SerializeField] private ProgressBar progressBar;
		[SerializeField] private BeerTap beerTap;
		[Tooltip("Timespan in s it takes to switch beerbarrels.")]
		[SerializeField] private float switchTime = 2f;

		private float switchingProgress = 0f;
		private bool isHolding;
		private PlayerMovement holdingPlayer;

		public override bool CanInteract(GameObject player, PickUp item) {
			if (isHolding) return false;
			return item is BeerBarrel && !beerTap.IsFull;
		}

		public override void Interact(GameObject player, PickUp barrel) {
			isHolding = true;
			StartCoroutine(SwitchBeerTapBarrel(barrel));

			holdingPlayer = player.GetComponent<PlayerMovement>();
			holdingPlayer.CanMove = false;
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			isHolding = false;
			holdingPlayer.CanMove = true;
		}

		private IEnumerator SwitchBeerTapBarrel(PickUp barrel) {
			progressBar.Show();

			while (switchingProgress <= switchTime && isHolding) {

				switchingProgress += Time.deltaTime;

				progressBar.UpdateProgress(switchingProgress / switchTime);

				if (switchingProgress > switchTime) {
					beerTap.Refill();

					BeerBarrel beerBarrel = barrel as BeerBarrel;
					beerBarrel.DropBarrel();

					Destroy(barrel.gameObject);

					switchingProgress = 0;
					progressBar.Hide();

					holdingPlayer.CanMove = true;

					break;
				}

				yield return null;
			}
		}
	}
}
