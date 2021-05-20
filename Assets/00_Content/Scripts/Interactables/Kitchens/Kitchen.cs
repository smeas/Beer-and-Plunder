using Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using Rounds;

namespace Interactables.Kitchens {
	public class Kitchen : Interactable {
		[SerializeField] ProgressBar cookingProgressBar;
		[SerializeField] Transform foodSpawnpoint;
		[SerializeField] GameObject foodPrefab;

		[Header("Settings")]
		[SerializeField] private float cookingTime = 10;

		private bool isCooking;
		private float cookingProgress = 0f;

		private void OnEnable() {
			if(RoundController.Instance != null) RoundController.Instance.OnRoundOver += HandleOnNewRoundStart;
		}

		private void OnDisable() {
			if(RoundController.Instance != null) RoundController.Instance.OnRoundOver -= HandleOnNewRoundStart;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			if (isCooking) return false;
			return item is KitchenTicket;
		}

		public override void Interact(GameObject player, PickUp item) {
			item.DropItem(player.transform);
			Destroy(item.gameObject);
			StartCoroutine(StartCooking());
		}

		private IEnumerator StartCooking() {
			cookingProgress = 0;
			isCooking = true;
			cookingProgressBar.Show();

			while (isCooking && cookingProgress <= cookingTime) {

				cookingProgress += Time.deltaTime;

				cookingProgressBar.UpdateProgress(cookingProgress / cookingTime);

				if (cookingProgress > cookingTime) {
					FinishCooking();
					break;
				}

				yield return null;
			}
		}

		private void FinishCooking() {
			isCooking = false;
			cookingProgressBar.Hide();
			Instantiate(foodPrefab, foodSpawnpoint);

			AudioManager.Instance.PlayEffect(SoundEffect.FoodReady);
		}
		/// <summary>
		/// Resets the progress etc on the kitchen between each round
		/// </summary>
		private void HandleOnNewRoundStart() {
			isCooking = false;
			//Might actually not need to reset cookingProgress to zero here, seeing as that is done whenever StartCooking() begins.
			cookingProgress = 0;
			cookingProgressBar.Hide();
		}
	}
}
