using System;
using System.Collections;
using Audio;
using Interactables;
using Interactables.Weapons;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace World {
	public class Goblin : MonoBehaviour, IHittable {
		[SerializeField] private Coin coinPrefab;
		[SerializeField] private float coinAttractionForce = 30f;
		[SerializeField] private float coinDropDelay = 0.2f;
		[SerializeField] private float fleeSpeedMultiplier = 2f;

		private NavMeshAgent agent;
		private Coin[] targets;
		private State state = State.None;
		private Vector3 exitPosition;
		private int currentTargetIndex = -1;

		public int Coins { get; set; }
		public bool CanPickUpCoins => state != State.Fleeing && state != State.None;

		public event Action<Goblin> OnLeave;

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
			else if (state == State.Leaving || state == State.Fleeing) {
				if (agent.pathPending) return;

				// No path or arrived
				if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.desiredVelocity == Vector3.zero) {
					state = State.None;

					if (state == State.Fleeing) {
						// Drop any excess coins
						while (Coins > 0)
							DropCoin();
					}

					OnLeave?.Invoke(this);
					Destroy(gameObject);
				}
			}
		}

		private void OnTriggerStay(Collider other) {
			if (!CanPickUpCoins) return;

			// Attract dem coins
			if (!other.isTrigger && other.CompareTag("Coin")) {
				Vector3 attractDirection = (transform.position - other.transform.position).normalized;
				other.attachedRigidbody.AddForce(attractDirection * coinAttractionForce);
			}
		}

		public void PickRandomTargetsAndGo(int targetCount, Vector3 exitPos) {
			exitPosition = exitPos;

			if (Coin.AllCoins.Count <= targetCount)
				targets = Coin.AllCoins.ToArray();
			else
				targets = Util.RandomSample(Coin.AllCoins, targetCount);

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

			AudioManager.PlayEffectSafe(SoundEffect.Goblin_GoblinLaugh);
		}

		void IHittable.Hit(Axe weapon) {
			if (state == State.Fleeing) return;

			// Flee
			StartCoroutine(CoDropCoins());
			agent.SetDestination(exitPosition);
			agent.speed *= fleeSpeedMultiplier;
			state = State.Fleeing;

			AudioManager.PlayEffectSafe(SoundEffect.Goblin_GoblinHit);
		}

		private IEnumerator CoDropCoins() {
			while (Coins > 0) {
				DropCoin();
				yield return new WaitForSeconds(coinDropDelay);
			}
		}

		private void DropCoin() {
			Instantiate(coinPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
			Coins--;
		}

		private enum State {
			None,
			Running,
			Leaving,
			Fleeing,
		}
	}
}