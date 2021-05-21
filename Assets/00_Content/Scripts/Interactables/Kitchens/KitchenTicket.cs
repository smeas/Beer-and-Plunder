namespace Interactables.Kitchens {
	public class KitchenTicket : PickUp {

		public override void HandleNewRoundReset() {
			base.HandleNewRoundReset();

			ShrinkAway();
		}
	}
}