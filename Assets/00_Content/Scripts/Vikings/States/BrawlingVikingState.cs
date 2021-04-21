using Interactables;
using UnityEngine;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is brawling
	/// </summary>
	public class BrawlingVikingState : VikingState {
		public BrawlingVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			if (BrawlController.Instance != null)
				BrawlController.Instance.EnterBrawl(viking);

			viking.bodyMeshRenderer.material = viking.brawlingMaterial;

			return this;
		}

		public override void Exit() {
			if (BrawlController.Instance != null)
				BrawlController.Instance.ExitBrawl(viking);

			viking.bodyMeshRenderer.material = viking.normalMaterial;
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			return this;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return true;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			viking.Stats.Reset();

			return new PassiveVikingState(viking);
		}
	}
}
