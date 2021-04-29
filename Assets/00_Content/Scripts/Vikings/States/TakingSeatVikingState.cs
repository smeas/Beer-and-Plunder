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

		public TakingSeatVikingState(Viking viking, Chair chair) : base(viking) {
			this.chair = chair;
		}

		public override VikingState Enter() {
			base.Enter();
			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			_ = navMeshAgent.SetDestination(chair.transform.position);

			return this;
		}

		public override void Exit() {
			base.Exit();
			navMeshAgent.enabled = false;
			viking.GetComponent<Rigidbody>().isKinematic = true;
		}

		public override VikingState Update() {
			if (navMeshAgent.enabled && navMeshAgent.desiredVelocity.sqrMagnitude <= 0.0001) {

				// Arrived at destination (mostly)
				Transform transform = viking.transform;
				transform.position = chair.SitPivot.position;
				transform.rotation = chair.SitPivot.rotation;

				return new PassiveVikingState(viking);
			}

			return this;
		}
	}
}