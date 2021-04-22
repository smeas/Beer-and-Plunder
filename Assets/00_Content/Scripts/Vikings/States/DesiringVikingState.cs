using Interactables;
using Player;
using UnityEngine;

namespace Vikings.States {
	public class DesiringVikingState : VikingState {
		public DesiringVikingState(Viking viking) : base(viking) {
			viking.bodyMeshRenderer.material = viking.desiringMaterial;
		}

		public override void Exit() {
			viking.bodyMeshRenderer.material = viking.normalMaterial;
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			return this;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			if (!(item is IDesirable givenItem)) return false;

			Debug.Assert(viking.CurrentDesire < viking.Desires.Length, "Viking is desiring more than it can");

			return givenItem.DesireType == viking.Desires[viking.CurrentDesire].type;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			viking.Stats.Reset();
			viking.CurrentDesire++;

			player.GetComponentInChildren<PlayerPickUp>().ConsumeItem();

			Object.Instantiate(viking.coinPrefab, viking.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

			if (viking.CurrentDesire >= viking.Desires.Length)
				return new LeavingVikingState(viking);

			return new PassiveVikingState(viking);
		}
	}
}
