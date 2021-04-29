namespace Vikings.States {
	public class PassiveVikingState : VikingState {
		public PassiveVikingState(Viking viking) : base(viking) {
		}

		public override VikingState Enter() {
			base.Enter();
			return this;
		}

		public override void Exit() {
			base.Exit();
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (viking.Stats.Mood < viking.Data.desiringMoodThreshold)
				return new DesiringVikingState(viking);

			return this;
		}
	}
}
