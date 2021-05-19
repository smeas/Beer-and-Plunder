using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Vikings.States {
	public class LeavingVikingState : VikingState {
		private float maxLeavingTimer;

		public LeavingVikingState(Viking viking) : base(viking) { }

		public override VikingState Enter() {

			if(viking.CurrentChair != null)
				viking.DismountChair();

			viking.NavMeshAgent.enabled = true;

			_ = viking.NavMeshAgent.SetDestination(VikingController.Instance != null
				                                ? VikingController.Instance.ExitPoint.position
				                                // Magic fallback exit position that may or may not work.
				                                : new Vector3(12f, 0, -8.5f));

			return this;
		}

		public override VikingState Update() {
			if (viking.NavMeshAgent.pathPending)
				return this;

			Debug.Assert(viking.NavMeshAgent.hasPath);

			maxLeavingTimer += Time.deltaTime;

			if (maxLeavingTimer >= viking.maxLeavingTime) {
				Debug.LogWarning("Viking took to long to leave the tavern", viking);
				Object.Instantiate(viking.disappearParticleSystem, viking.transform.position + new Vector3(0, 0.8f, 0), viking.transform.rotation)
					.gameObject.AddComponent<ParticleCleanup>();

				viking.FinishLeaving();
				return new NullVikingState(viking);
			}

			if (viking.NavMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) {
				Debug.LogWarning("Viking has no exit path or is blocked!", viking);
				// What should we do here? Forcing a leave for now to avoid a softlock.
				viking.FinishLeaving();
				return new NullVikingState(viking);
			}

			if (viking.NavMeshAgent.desiredVelocity.sqrMagnitude == 0f) {
				viking.FinishLeaving();
				return new NullVikingState(viking);
			}

			return this;
		}
	}
}
