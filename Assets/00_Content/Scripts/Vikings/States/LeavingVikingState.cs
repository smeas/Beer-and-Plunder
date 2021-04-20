using UnityEngine;
using UnityEngine.AI;

namespace Vikings.States {
	public class LeavingVikingState : VikingState {
		private NavMeshAgent navMeshAgent;

		public LeavingVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			// If we're sitting on a chair, dismount it.
			if (viking.CurrentChair != null) {
				viking.transform.position = viking.CurrentChair.DismountPoint.position;
				viking.CurrentChair.OnVikingLeaveChair(viking);
				viking.CurrentChair = null;
			}

			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			_ = navMeshAgent.SetDestination(VikingController.Instance != null
				                                ? VikingController.Instance.ExitPoint.position
				                                // Magic fallback exit position that may or may not work.
				                                : new Vector3(12f, 0, -8.5f));

			return this;
		}

		public override VikingState Update() {
			if (!navMeshAgent.hasPath) {
				Debug.LogWarning("Viking has no exit path!", viking);
				// What should we do here? Forcing a leave for now to avoid a softlock.
				viking.FinishLeaving();
			}

			if (navMeshAgent.desiredVelocity.sqrMagnitude == 0f)
				viking.FinishLeaving();

			return this;
		}
	}
}