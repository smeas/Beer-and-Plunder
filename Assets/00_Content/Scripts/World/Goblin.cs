using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Extensions;
using Interactables;
using Interactables.Weapons;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Random = UnityEngine.Random;

namespace World {
	public class Goblin : MonoBehaviour, IHittable {
		[SerializeField] private float coinAttractionForce = 30f;
		[SerializeField] private float coinDropDelay = 0.2f;
		[SerializeField] private float fleeSpeedMultiplier = 2f;
		[SerializeField] private float maxLeaveTime = 8f;

		[Header("Coin stack")]
		[SerializeField] private Transform coinStackPosition;
		[SerializeField] private float coinHeight;
		[SerializeField] private float coinDisplacement = 0.08f;

		[Header("Effects")]
		[SerializeField] private GameObject spawnEffectPrefab;

		private NavMeshAgent agent;
		private Coin[] targets;
		private State state = State.None;
		private Vector3 exitPosition;
		private int currentTargetIndex = -1;
		private float leaveTimer;

		private Transform coinRoot;
		private List<Coin> carriedCoins = new List<Coin>();

		public int Coins => carriedCoins.Count;
		public bool CanPickUpCoins => state != State.Fleeing && state != State.None;

		public event Action<Goblin> OnLeave;

		private void Awake() {
			agent = GetComponent<NavMeshAgent>();

			coinRoot = new GameObject("Coins").transform;
			coinRoot.SetParent(transform);
		}

		private void Start() {
			SpawnPoofCloud();
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

				leaveTimer += Time.deltaTime;

				// No path or arrived
				if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.desiredVelocity == Vector3.zero || leaveTimer >= maxLeaveTime) {
					FinishLeaving();
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

		private void FinishLeaving() {
			if (state == State.Fleeing) {
				// Drop any excess coins
				while (Coins > 0)
					DropCoin();
			}

			state = State.None;
			OnLeave?.Invoke(this);
			Destroy(gameObject);
			SpawnPoofCloud();
		}

		private void SpawnPoofCloud() {
			Instantiate(spawnEffectPrefab, transform.position + new Vector3(0, 0.8f, 0), transform.rotation)
				.AddComponent<ParticleCleanup>();
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

		public bool PickUpCoin(Coin coin) {
			if (!CanPickUpCoins) return false;

			Vector2 displacement = carriedCoins.Count > 0
				? new Vector2(Random.value * coinDisplacement, Random.value * coinDisplacement)
				: Vector2.zero;
			Transform coinTransform = coin.transform;

			coin.IsDisplay = true;
			coinTransform.SetParent(coinRoot);
			coinTransform.rotation = Quaternion.identity;
			coinTransform.position = coinStackPosition.position +
				new Vector3(displacement.x, carriedCoins.Count * coinHeight, displacement.y);
			carriedCoins.Add(coin);

			return true;
		}

		private void DropCoin() {
			if (carriedCoins.Count <= 0) return;

			Coin coin = carriedCoins.Pop();
			coin.transform.SetParent(null);
			coin.IsDisplay = false;
			coin.RandomThrow();
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
					Leave();
				}

				break;
			}

			AudioManager.PlayEffectSafe(SoundEffect.Goblin_GoblinLaugh);
		}

		void IHittable.Hit(Axe weapon) {
			if (state == State.Fleeing) return;

			Flee();
			AudioManager.PlayEffectSafe(SoundEffect.Goblin_GoblinHit);
		}

		private IEnumerator CoDropAllCoins() {
			while (Coins > 0) {
				DropCoin();
				yield return new WaitForSeconds(coinDropDelay);
			}
		}

		private void Flee() {
			StartCoroutine(CoDropAllCoins());
			agent.SetDestination(exitPosition);
			agent.speed *= fleeSpeedMultiplier;
			state = State.Fleeing;
		}

		public void Leave() {
			agent.SetDestination(exitPosition);
			state = State.Leaving;
		}

		private enum State {
			None,
			Running,
			Leaving,
			Fleeing,
		}
	}
}