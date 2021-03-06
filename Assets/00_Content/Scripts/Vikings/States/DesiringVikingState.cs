using Audio;
using Interactables;
using Interactables.Beers;
using Player;
using UnityEngine;
using Utilities;

namespace Vikings.States {
	public class DesiringVikingState : VikingState {
		private bool hasActiveFulfillment;
		private float fulfillmentTimer;
		private GameObject fulfillingPlayer;
		private bool isOrderGiven;
		private bool isIrritaded;

		public DesiringVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			if (viking.CurrentDesire.type != DesireType.Null) {
				viking.animationDriver.TriggerRequest();
				viking.desireVisualiser.ShowNewDesire(viking.CurrentDesire.visualisationSprite);
				PlayDesireAudio(viking.CurrentDesire.type);
			}

			return this;
		}

		public override void Exit() {
			viking.desireVisualiser.HideDesire();
			viking.progressBar.Hide();
		}

		public override VikingState Update() {
			float remappedMood = MathX.RemapClamped(viking.Stats.Mood, viking.Data.brawlMoodThreshold, viking.Stats.StartMood, 0, 1);
			viking.desireVisualiser.SetDesireColor(remappedMood);
			viking.desireVisualiser.SetTweenSpeed(remappedMood);

			if(viking.Stats.Mood <= 30 && !isIrritaded) {
				AudioManager.PlayEffectSafe(SoundEffect.Viking_Desire_Irritated);
				isIrritaded = true;
			}

			if (viking.Stats.Mood > 30)
				isIrritaded = false;

			if (hasActiveFulfillment) {
				fulfillmentTimer += Time.deltaTime;

				if (fulfillmentTimer >= viking.CurrentDesire.desireFulfillTime)
					return DesireFulfilled();

				viking.progressBar.UpdateProgress(fulfillmentTimer / viking.CurrentDesire.desireFulfillTime);

				return this;
			}

			viking.Stats.Decline();

			if (viking.Stats.Mood < viking.Data.brawlMoodThreshold && viking.Data.canStartBrawl)
				return new BrawlingVikingState(viking, viking.CurrentChair.Table);

			return this;
		}

		public override void Affect(GameObject player, PickUp item) {
			if (viking.CurrentDesire.isMaterialDesire) return;
			if (hasActiveFulfillment) return;
			if (!(item is IDesirable desiredItem)) return;
			if (desiredItem.DesireType != viking.Desires[viking.CurrentDesireIndex].type) return;

			fulfillmentTimer = 0;
			hasActiveFulfillment = true;
			fulfillingPlayer = player;

			if (viking.CurrentDesire.desireFulfillTime != 0)
				viking.progressBar.Show();
		}

		public override void CancelAffect(GameObject player, PickUp item) {
			if (player != fulfillingPlayer) return;

			hasActiveFulfillment = false;
			viking.progressBar.Hide();
			fulfillingPlayer = null;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			Debug.Assert(viking.CurrentDesireIndex < viking.Desires.Length, "Viking is desiring more than it can");

			if (viking.CurrentDesire.isOrder && !isOrderGiven) return item == null;
			if (!(item is IDesirable givenItem)) return false;
			if (!viking.CurrentDesire.isMaterialDesire) return false;
			if (hasActiveFulfillment) return false;

			if (givenItem.DesireType != viking.CurrentDesire.type) return false;
			if (item is Tankard {IsFull: false})
				return false;

			return true;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			fulfillingPlayer = player;

			if (viking.CurrentDesire.isOrder && !isOrderGiven) {
				SpawnOrderTicket(player);
				viking.Stats.BoostMood(viking.Data.moodBoostDesireFulfilled);
				viking.OrderTaken?.Invoke();
				return this;
			}

			if (viking.CurrentDesire.desireFulfillTime == 0)
				return DesireFulfilled();

			hasActiveFulfillment = true;
			fulfillmentTimer = 0;
			viking.progressBar.Show();
			fulfillingPlayer.GetComponentInChildren<PlayerMovement>().BlockMovement();

			return this;
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (player != fulfillingPlayer) return;

			fulfillingPlayer.GetComponentInChildren<PlayerMovement>().UnblockMovement();

			hasActiveFulfillment = false;
			fulfillingPlayer = null;
			viking.progressBar.Hide();
		}

		private VikingState DesireFulfilled() {
			DesireData desire = viking.CurrentDesire;

			viking.CurrentDesireIndex++;
			viking.MoodWhenDesireFulfilled.Add(viking.Stats.Mood);
			viking.Stats.BoostMood(viking.Data.moodBoostDesireFulfilled);
			AudioManager.PlayEffectSafe(SoundEffect.Viking_Desire_DesireFullfilled);

			if (desire.isMaterialDesire) {
				PlayerPickUp playerPickUp = fulfillingPlayer.GetComponentInChildren<PlayerPickUp>();
				PickUp givenItem = playerPickUp.PickedUpItem;

				Debug.Assert(givenItem != null, "Viking was given item null");

				playerPickUp.DropItem();
				givenItem.VikingPickUpItem(viking);

				fulfillingPlayer.GetComponentInChildren<PlayerMovement>().UnblockMovement();
				return new SatisfiedVikingState(viking, desire, givenItem);
			}

			return new SatisfiedVikingState(viking, desire);
		}

		private void SpawnOrderTicket(GameObject player) {
			viking.desireVisualiser.ShowNewDesire(viking.CurrentDesire.visualisationAfterPrefab);

			PickUp ticket = Object.Instantiate(viking.kitchenTicketPrefab, viking.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
			player.GetComponentInChildren<PlayerPickUp>().TryReceiveItem(ticket);
			isOrderGiven = true;
		}

		private void PlayDesireAudio(DesireType type) {
			switch (type) {
				case DesireType.Beer:
					AudioManager.PlayEffectSafe(SoundEffect.Viking_Desire_NeedBeer);
					break;
				case DesireType.Harp:
					AudioManager.PlayEffectSafe(SoundEffect.Viking_Desire_NeedMusic);
					break;
				case DesireType.Food:
					AudioManager.PlayEffectSafe(SoundEffect.Viking_Desire_NeedFood);
					break;
			}
		}
	}
}
