namespace Vikings {
	public class DesiringVikingState : VikingState {
		public DesiringVikingState(Viking viking) : base(viking) {
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			return this;
		}

		public override VikingState GiveItem() {
			return new PassiveVikingState(viking);
		}
	}
}
