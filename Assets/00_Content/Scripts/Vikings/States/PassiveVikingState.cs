using UnityEngine;

namespace Vikings.States {
	public class PassiveVikingState : VikingState {
		private float intervalDuration;
		private float intervalTimer;

		public PassiveVikingState(Viking viking) : base(viking) {
		}

		public override VikingState Enter() {
			intervalDuration = Random.Range(viking.Data.desireIntervalMinMax.x, viking.Data.desireIntervalMinMax.y);
			intervalTimer = intervalDuration;

			return this;
		}

		public override VikingState Update() {
			intervalTimer -= Time.deltaTime;

			if (intervalTimer <= 0)
				return new DesiringVikingState(viking);

			return this;
		}
	}
}
