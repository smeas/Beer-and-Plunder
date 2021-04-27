using Interactables;
using Player;
using UnityEngine;

namespace Vikings.States {
	public class DesiringVikingState : VikingState {
		public DesiringVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			viking.desireVisualiser.ShowNewDesire(viking.CurrentDesire.visualisationPrefab);

			return this;
		}

		public override void Exit() {
			viking.desireVisualiser.HideDesire();
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (viking.Stats.Mood < viking.Data.brawlMoodThreshold && viking.Data.canStartBrawl)
				return new BrawlingVikingState(viking, viking.CurrentChair.Table);

			return this;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			if (!(item is IDesirable givenItem)) return false;

			Debug.Assert(viking.CurrentDesireIndex < viking.Desires.Length, "Viking is desiring more than it can");

			return givenItem.DesireType == viking.CurrentDesire.type;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			viking.Stats.Reset();
			viking.CurrentDesireIndex++;

			player.GetComponentInChildren<PlayerPickUp>().ConsumeItem();

			Object.Instantiate(viking.coinPrefab, viking.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

			if (viking.CurrentDesireIndex >= viking.Desires.Length)
				return new LeavingVikingState(viking);

			return new PassiveVikingState(viking);
		}
	}
}
