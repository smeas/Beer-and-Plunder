using Interactables;
using UnityEngine;

namespace Vikings.States {
	public class DesiringVikingState : VikingState {
		public DesiringVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			viking.bodyMeshRenderer.material = viking.desiringMaterial;

			return this;
		}

		public override void Exit() {
			viking.bodyMeshRenderer.material = viking.normalMaterial;
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (viking.Stats.Mood < viking.Data.brawlMoodThreshold && viking.Data.canStartBrawl)
				return new BrawlingVikingState(viking, viking.CurrentChair.Table);

			return this;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return true;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			viking.Stats.Reset();
			viking.Desires--;

			Object.Instantiate(viking.coinPrefab, viking.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

			if (viking.Desires <= 0)
				return new LeavingVikingState(viking);

			return new PassiveVikingState(viking);
		}
	}
}
