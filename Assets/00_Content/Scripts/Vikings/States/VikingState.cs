using Interactables;
using Interactables.Weapons;
using System;
using UnityEngine;

namespace Vikings.States {
	public abstract class VikingState {
		protected Viking viking;

		protected VikingState(Viking viking) {
			this.viking = viking;
		}

		public virtual VikingState Enter() {
			return this;
		}

		public virtual VikingState HandleOnHit(Axe axe, Viking viking) {
			viking.Stats.TakeMoodDamage(axe.WeaponData.moodDamage);

			if (viking.Stats.Mood <= viking.Data.brawlMoodThreshold) {
				return new BrawlingVikingState(viking, axe.GetComponentInParent<Player.PlayerComponent>());
			}
			return this;

		}

		public virtual VikingState Update() {
			return this;
		}

		public virtual void Exit() {
		}

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