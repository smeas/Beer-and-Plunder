﻿using Interactables;
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
			return true;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			viking.Stats.Reset();
			viking.Desires--;

			if (viking.Desires <= 0)
				return new LeavingVikingState(viking);

			return new PassiveVikingState(viking);
		}
	}
}
