using Interactables;
using Player;
using UnityEngine;

namespace Vikings {
	/// <summary>
	/// State for when the viking is waiting to be seated - standing in queue at the door.
	/// </summary>
	public class WaitingForSeatVikingState : VikingState {
		public WaitingForSeatVikingState(Viking viking) : base(viking) { }

		public override VikingState Update() {
			return this;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return player.GetComponent<PlayerSteward>().Follower == null;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			player.GetComponent<PlayerSteward>().BeginSeatingViking(viking);
			return new BeingSeatedVikingState(viking, player);
		}
	}
}