using Interactables;
using Player;
using UnityEngine;

namespace Vikings.States {
	public class DesiringVikingState : VikingState {
		private bool hasActiveFulfillment;
		private float fulfillmentTimer;
		private GameObject fulfillingPlayer;

		public DesiringVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			viking.desireVisualiser.ShowNewDesire(viking.CurrentDesire.visualisationPrefab);

			return this;
		}

		public override void Exit() {
			viking.desireVisualiser.HideDesire();
			viking.progressBar.Hide();
		}

		public override VikingState Update() {
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
			if (!(item is IDesirable givenItem)) return false;
			if (!viking.CurrentDesire.isMaterialDesire) return false;
			if (hasActiveFulfillment) return false;

			Debug.Assert(viking.CurrentDesireIndex < viking.Desires.Length, "Viking is desiring more than it can");

			return givenItem.DesireType == viking.CurrentDesire.type;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			fulfillingPlayer = player;

			if (viking.CurrentDesire.desireFulfillTime == 0)
				return DesireFulfilled();

			hasActiveFulfillment = true;
			fulfillmentTimer = 0;
			viking.progressBar.Show();
			fulfillingPlayer.GetComponentInChildren<PlayerMovement>().CanMove = false;

			return this;
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (player != fulfillingPlayer) return;

			fulfillingPlayer.GetComponentInChildren<PlayerMovement>().CanMove = true;

			hasActiveFulfillment = false;
			fulfillingPlayer = null;
			viking.progressBar.Hide();
		}

		private VikingState DesireFulfilled() {
			if (viking.CurrentDesire.isMaterialDesire) {
				fulfillingPlayer.GetComponentInChildren<PlayerPickUp>().ConsumeItem();
				fulfillingPlayer.GetComponentInChildren<PlayerMovement>().CanMove = true;
			}

			viking.CurrentDesireIndex++;
			viking.MoodWhenDesireFulfilled.Add(viking.Stats.Mood);

			return new SatisfiedVikingState(viking);
		}
	}
}
