using Interactables;
using UnityEngine;

namespace Vikings {
	public abstract class VikingState {
		protected Viking viking;

		protected VikingState(Viking viking) {
			this.viking = viking;
		}

		public virtual VikingState Enter() {
			return this;
		}

		public virtual VikingState Update() {
			return this;
		}

		public virtual void Exit() { }


		public virtual bool CanInteract(GameObject player, PickUp item) {
			return false;
		}

		public virtual VikingState Interact(GameObject player, PickUp item) {
			return this;
		}

		public virtual VikingState TakeSeat(Chair chair) {
			return this;
		}
	}
}