using Interactables;
using Player;
using UnityEngine;

namespace Vikings {
	/// <summary>
	/// State for when the viking is being seated by a player - the player is leading the viking to their chair.
	/// </summary>
	public class BeingSeatedVikingState : VikingState {
		private readonly GameObject player;
		private GameObject highlight;

		public BeingSeatedVikingState(Viking viking, GameObject player) : base(viking) {
			this.player = player;
		}

		public override VikingState Enter() {
			highlight = Object.Instantiate(viking.beingSeatedHighlightPrefab, viking.transform);
			highlight.transform.localPosition = Vector3.up * 2;

			return this;
		}

		public override void Exit() {
			Object.Destroy(highlight);
		}

		// TODO: Follow the player.
		public override VikingState Update() {
			return this;
		}

		public override VikingState TakeSeat(Chair chair) {
			player.GetComponent<PlayerSteward>().EndSeatingViking(viking);
			return new TakingSeatVikingState(viking, chair);
		}
	}
}