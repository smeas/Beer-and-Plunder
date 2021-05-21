using System;
using Audio;
using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;
using Rounds;

namespace Interactables.Kitchens {
	public class Kitchen : Interactable {
		[SerializeField] private ProgressBar cookingProgressBar;
		[SerializeField] private Transform foodSpawnpoint;
		[SerializeField] private ParticleSystem foodSpawnEffect;
		[SerializeField] private Food foodPrefab;
		[SerializeField] private ParticleSystem smokeParticleSystem;

		[Header("Settings")]
		[SerializeField] private float cookingTime = 10;

		private Queue<KitchenTicket> tickets = new Queue<KitchenTicket>();
		private bool isCooking;
		private SoundHandle cookingSound;

		public event Action<Food> CookingFinished;

		private void OnEnable() {
			if(RoundController.Instance != null) RoundController.Instance.OnRoundOver += HandleOnNewRoundStart;
		}

		private void OnDisable() {
			if(RoundController.Instance != null) RoundController.Instance.OnRoundOver -= HandleOnNewRoundStart;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return item is KitchenTicket;
		}

		public override void Interact(GameObject player, PickUp item) {
			player.GetComponentInChildren<PlayerPickUp>().DropItem();
			item.gameObject.SetActive(false);

			Debug.Assert(item is KitchenTicket, "Kitchen is given a non ticket");

			StartCoroutine(StartCooking((KitchenTicket)item));
		}

		private IEnumerator StartCooking(KitchenTicket ticket) {
			tickets.Enqueue(ticket);
			float cookingProgress = 0;
			isCooking = true;
			cookingProgressBar.Show();
			smokeParticleSystem.Play(true);

			if(tickets.Count == 1) {
				cookingSound = AudioManager.Instance.PlayEffect(SoundEffect.Cooking, true);
			}

			while (isCooking && cookingProgress <= cookingTime) {

				cookingProgress += Time.deltaTime;

				if (ticket == tickets.Peek())
					cookingProgressBar.UpdateProgress(cookingProgress / cookingTime);

				if (cookingProgress > cookingTime) {
					FinishCooking();
					break;
				}

				yield return null;
			}
		}

		private void FinishCooking() {
			Destroy(tickets.Dequeue());

			if (tickets.Count == 0) {
				cookingProgressBar.Hide();
				isCooking = false;
				smokeParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				cookingSound.Stop();
			}

			Food food = Instantiate(foodPrefab, foodSpawnpoint);
			foodSpawnEffect.Play();

			AudioManager.Instance.PlayEffect(SoundEffect.FoodReady);

			CookingFinished?.Invoke(food);
		}
		/// <summary>
		/// Resets the progress etc on the kitchen between each round
		/// </summary>
		private void HandleOnNewRoundStart() {
			isCooking = false;
			StopAllCoroutines();

			while(tickets.Count > 0) {
				Destroy(tickets.Dequeue());
			}

			cookingProgressBar.Hide();
		}
	}
}
