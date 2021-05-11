using Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace Interactables.Kitchens {
	public class Kitchen : Interactable {
		[SerializeField] ProgressBar cookingProgressBar;
		[SerializeField] Transform foodSpawnpoint;
		[SerializeField] GameObject foodPrefab;

		[Header("Settings")]
		[SerializeField] private float cookingTime = 10;

		private bool isCooking;
		private float cookingProgress = 0f;

		public override void CancelInteraction(GameObject player, PickUp item) {
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

			Debug.Log("Food is ready!");
			AudioManager.Instance.PlayEffect(SoundEffect.FoodReady);
		}
	}
}
