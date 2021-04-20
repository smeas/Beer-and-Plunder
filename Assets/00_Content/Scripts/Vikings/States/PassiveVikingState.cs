namespace Vikings.States {
	public class PassiveVikingState : VikingState {
		public PassiveVikingState(Viking viking) : base(viking) {
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (viking.Stats.Mood < 50)
				return new DesiringVikingState(viking);

			return this;
		}
	}
}
