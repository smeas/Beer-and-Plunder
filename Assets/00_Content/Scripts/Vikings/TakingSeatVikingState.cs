using Interactables;
using UnityEngine;

namespace Vikings {
	/// <summary>
	/// State for when the viking has been assigned a chair and is going to sit on it.
	/// </summary>
	public class TakingSeatVikingState : VikingState {
		private readonly Chair chair;

		public TakingSeatVikingState(Viking viking, Chair chair) : base(viking) {
			this.chair = chair;
		}

		public override VikingState Enter() {
			viking.CurrentChair = chair;
			chair.OnVikingTakeChair(viking);

			Transform transform = viking.transform;
			transform.position = chair.SitPivot.position;
			transform.rotation = chair.SitPivot.rotation;

			viking.GetComponent<Rigidbody>().isKinematic = true;

			return new PassiveVikingState(viking);
		}
	}
}