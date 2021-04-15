namespace Vikings {
	public abstract class VikingState {
		protected Viking viking;

		protected VikingState(Viking viking) {
			this.viking = viking;
		}

		public abstract VikingState Update();

		public virtual VikingState GiveItem() {
			return this;
		}
	}
}
