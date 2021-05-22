using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Vikings.States {
	public class LeavingVikingState : VikingState {
		private float maxLeavingTimer;
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
			maxLeavingTimer += Time.deltaTime;

			if (maxLeavingTimer >= viking.maxLeavingTime) {
				Debug.LogWarning("Viking took to long to leave the tavern", viking);
				Object.Instantiate(viking.disappearParticleSystem, viking.transform.position + new Vector3(0, 0.8f, 0), viking.transform.rotation)
					.gameObject.AddComponent<ParticleCleanup>();

				viking.FinishLeaving();
				return new NullVikingState(viking);
			}

			if (isDismounting) {
				if (!viking.animationDriver.IsSitting) {
					StartNavigating();
					isDismounting = false;
				}
				else {
					return this;
				}
			}

			if (viking.NavMeshAgent.pathPending)
				return this;

			Debug.Assert(viking.NavMeshAgent.hasPath);

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

		private void StartNavigating() {
			viking.NavMeshAgent.enabled = true;

			_ = viking.NavMeshAgent.SetDestination(VikingController.Instance != null
				                                       ? VikingController.Instance.ExitPoint.position
				                                       // Magic fallback exit position that may or may not work.
				                                       : new Vector3(12f, 0, -8.5f));

			isDismounting = false;
		}
	}
}
