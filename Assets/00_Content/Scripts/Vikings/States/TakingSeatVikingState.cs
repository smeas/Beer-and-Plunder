using Interactables;
using UnityEngine;
using UnityEngine.AI;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking has been assigned a chair and is going to sit on it.
	/// </summary>
	public class TakingSeatVikingState : VikingState {
		private readonly Chair chair;
		private NavMeshAgent navMeshAgent;
		private bool isSeating;

		public TakingSeatVikingState(Viking viking, Chair chair) : base(viking) {
			this.chair = chair;
		}

		public override VikingState Enter() {
			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			_ = navMeshAgent.SetDestination(chair.transform.position);

			return this;
		}

		public override void Exit() {
			if (isSeating)
				viking.animationDriver.InterruptSeating();
		}

		public override VikingState Update() {
			if (isSeating) {
				if (viking.animationDriver.IsSitting)
					return new PassiveVikingState(viking);
			}
			else if (navMeshAgent.enabled && navMeshAgent.desiredVelocity.sqrMagnitude <= 0.0001f) {
				navMeshAgent.enabled = false;
				viking.GetComponent<Rigidbody>().isKinematic = true;

				viking.animationDriver.BeginSitting(chair.SitPivot.position, chair.Table.transform.position);
				isSeating = true;
			}

			return this;
		}
	}
}