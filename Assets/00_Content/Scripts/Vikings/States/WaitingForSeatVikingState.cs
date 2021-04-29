using System.Linq;
using Interactables;
using Interactables.Weapons;
using Player;
using UnityEngine;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is waiting to be seated - standing in queue at the door.
	/// </summary>
	public class WaitingForSeatVikingState : VikingState {
		public WaitingForSeatVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			return this;
		}

		public override VikingState HandleOnHit(Axe axe, Viking viking) {
			return new LeavingVikingState(viking);
		}

		public override void Exit() {
			viking.FinishQueueing();
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (viking.Stats.Mood < viking.Data.impatientMoodThreshold)
				return TakeRandomSeat();

			return this;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return viking.QueuePosition == 0
				&& player.GetComponent<PlayerSteward>().Follower == null;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			player.GetComponent<PlayerSteward>().BeginSeatingViking(viking);
			return new BeingSeatedVikingState(viking, player);
		}

		private VikingState TakeRandomSeat() {
			if (Table.AllTables.Count == 0)
				return this;

			Table[] freeTables = Table.AllTables.Where(tbl => !tbl.IsFull && !tbl.IsDestroyed).ToArray();
			if (freeTables.Length == 0)
				return this;

			Table table = freeTables[Random.Range(0, freeTables.Length)];
			Chair[] freeChairs = table.Chairs.Where(chr => !chr.IsOccupied).ToArray();
			if (freeChairs.Length == 0)
				return this;

			Chair chair = freeChairs[Random.Range(0, freeChairs.Length)];
			viking.CurrentChair = chair;
			chair.OnVikingTakeChair(viking);

			return new TakingSeatVikingState(viking, chair);
		}
	}
}
