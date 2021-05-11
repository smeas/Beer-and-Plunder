using System.Linq;
using Interactables;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace World {
	public class Goblin : MonoBehaviour {
		[SerializeField] private float coinAttractionForce = 30f;

		private NavMeshAgent agent;
		private Coin[] targets;
		private State state = State.None;
		private Vector3 exitPosition;
		private int currentTargetIndex = -1;

		private void Awake() {
			agent = GetComponent<NavMeshAgent>();
		}

		private void Update() {
			if (state == State.Running) {
				if (agent.pathPending) return;

				// No path found or coin is gone
				if (agent.pathStatus != NavMeshPathStatus.PathComplete || targets[currentTargetIndex] == null) {
					NextTarget();
					return;
				}

				// Arrived at target
				if (agent.desiredVelocity == Vector3.zero)
					NextTarget();
			}
			else if (state == State.Leaving) {
				if (agent.pathPending) return;

				// No path or arrived
				if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.desiredVelocity == Vector3.zero) {
					state = State.None;
					Destroy(gameObject);
				}
			}
		}

		public void PickRandomTargetsAndGo(int targetCount, Vector3 exitPos) {
			exitPosition = exitPos;

			if (Coin.AllCoins.Count <= targetCount)
				targets = Coin.AllCoins.ToArray();
			else
				targets = Util.RandomSample(Coin.AllCoins.ToArray(), targetCount);

			currentTargetIndex = -1;
			NextTarget();
		}

		private void NextTarget() {
			while (true) {
				currentTargetIndex++;
				if (currentTargetIndex < targets.Length) {
					if (targets[currentTargetIndex] == null)
						continue;

					agent.SetDestination(targets[currentTargetIndex].transform.position);
					state = State.Running;
				}
				else {
					agent.SetDestination(exitPosition);
					state = State.Leaving;
				}

				break;
			}
		}

		private void OnTriggerStay(Collider other) {
			// Attract dem coins
			if (!other.isTrigger && other.CompareTag("Coin")) {
				Vector3 attractDirection = (transform.position - other.transform.position).normalized;
				other.attachedRigidbody.AddForce(attractDirection * coinAttractionForce);
			}
		}

		private enum State {
			None,
			Running,
			Leaving,
		}
	}
}