using UnityEngine;
using UnityEngine.AI;

namespace Vikings.States {
	public class LeavingVikingState : VikingState {
		private NavMeshAgent navMeshAgent;
		private bool isDismounting;

		public LeavingVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {
			if (viking.CurrentChair != null) {
				isDismounting = true;
				viking.DismountChair();
			}
			else {
				StartNavigating();
			}

			return this;
		}

		public override VikingState Update() {
			if (isDismounting) {
				if (!viking.animationDriver.IsSitting) {
					StartNavigating();
					isDismounting = false;
				}
				else {
					return this;
				}
			}

			if (navMeshAgent.pathPending)
				return this;

			Debug.Assert(navMeshAgent.hasPath);

			if (navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) {
				Debug.LogWarning("Viking has no exit path or is blocked!", viking);
				// What should we do here? Forcing a leave for now to avoid a softlock.
				viking.FinishLeaving();
				return new NullVikingState(viking);
			}

			if (navMeshAgent.desiredVelocity.sqrMagnitude == 0f) {
				viking.FinishLeaving();
				return new NullVikingState(viking);
			}

			return this;
		}

		private void StartNavigating() {
			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			_ = navMeshAgent.SetDestination(VikingController.Instance != null
				                                ? VikingController.Instance.ExitPoint.position
				                                // Magic fallback exit position that may or may not work.
				                                : new Vector3(12f, 0, -8.5f));

			isDismounting = false;
		}
	}
}
