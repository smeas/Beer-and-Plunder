using Extensions;
using Interactables;
using Interactables.Weapons;
using Player;
using Rounds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Vikings.States;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public delegate void VikingLeavingQueue(Viking sender);

	public class Viking : Interactable {
		[SerializeField] private VikingData vikingData;
		[SerializeField] private LayerMask weaponLayer;
		[SerializeField] public GameObject beingSeatedHighlightPrefab;
		[SerializeField] public Coin coinPrefab;
		[SerializeField] public MeshRenderer bodyMeshRenderer;
		[SerializeField] public List<MeshRenderer> hitHighlightMeshRenderers;
		[SerializeField] public Material normalMaterial;
		[SerializeField] public Material desiringMaterial;
		[SerializeField] public Material brawlingMaterial;

		private bool debugModeStarted;
		private VikingState state;
		private VikingScaling statScaling;
		private Rigidbody rb;
		private NavMeshAgent navMeshAgent;
		private bool isAttacked;
		private bool isAttacking;

		public VikingData Data => vikingData;
		public DesireData[] Desires => vikingData.desires;
		public VikingStats Stats { get; private set; }
		public Chair CurrentChair { get; set; }
		public int CurrentDesireIndex { get; set; }
		public int QueuePosition { get; set; }

		public bool IsSeated { get; set; }
		public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

		public event VikingLeaving LeaveTavern;
		public event VikingLeavingQueue LeaveQueue;

		private void Start() {
			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();
			rb = GetComponent<Rigidbody>();
			navMeshAgent = GetComponent<NavMeshAgent>();

			Stats = new VikingStats(vikingData, statScaling);

			ChangeState(new WaitingForSeatVikingState(this));

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += HandleOnRoundOver;
		}

		private void Update() {

			if (Data.defaultAttackPlayer && !debugModeStarted) {
				var player = PlayerManager.Instance.Players.FirstOrDefault();

				if (player == null)
					return;

				ChangeState(new BrawlingVikingState(this, player));
				debugModeStarted = true;
			}

			ChangeState(state.Update());
		}

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= HandleOnRoundOver;
		}

		private bool ChangeState(VikingState newState) {
			if (newState == state)
				return false;

			do {
				state?.Exit();
				state = newState;
				newState = state.Enter();
			} while (newState != state);

			return true;
		}

		private void HandleOnRoundOver() {
			// Leave when the round is over.
			if (!(state is LeavingVikingState))
				ChangeState(new LeavingVikingState(this));
		}

		public void SetScaling(VikingScaling scaling) {
			statScaling = scaling;
		}

		public bool TryTakeSeat(Chair chair) {
			return ChangeState(state.TakeSeat(chair));
		}

		public void DismountChair() {
			if (CurrentChair == null) return;

			transform.position = CurrentChair.DismountPoint.position;
			CurrentChair.OnVikingLeaveChair(this);
			CurrentChair = null;
		}

		public void JoinBrawl() {
			ChangeState(new BrawlingVikingState(this, CurrentChair.Table));
		}

		public void MakeSpinAttack() {
			IsAttacking = true;
			StartCoroutine(SpinAttack());
			Invoke(nameof(FinishAttack), vikingData.spinAttackDuration);
		}

		private IEnumerator SpinAttack() {
			while(IsAttacking) {
				yield return null;
				transform.Rotate(Vector3.up, vikingData.spinAttackSpeed * Time.deltaTime);
			}
		}

		private void FinishAttack() {
			StopAllCoroutines();
			IsAttacking = false;
			isAttacked = false;

			foreach (MeshRenderer hitHighlightMeshRenderer in hitHighlightMeshRenderers) {
				hitHighlightMeshRenderer.material = normalMaterial;
			}
		}

		public void FinishLeaving() {
			LeaveTavern?.Invoke(this);

			if (CurrentChair != null) {
				CurrentChair.OnVikingLeaveChair(this);
				CurrentChair = null;
			}

			if (VikingController.Instance == null)
				Destroy(gameObject);
		}

		public void FinishQueueing() {
			LeaveQueue?.Invoke(this);
		}

		public override void Interact(GameObject player, PickUp item) {
			ChangeState(state.Interact(player, item));
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return state.CanInteract(player, item);
		}

		private void OnTriggerEnter(Collider other) {

			if (weaponLayer.ContainsLayer(other.gameObject.layer)) {

				if (state is LeavingVikingState)
					return;

				Axe axe = other.gameObject.GetComponent<Axe>();
				if (axe.IsAttacking && !isAttacked) {
					isAttacked = true;

					foreach (MeshRenderer hitHighlightMeshRenderer in hitHighlightMeshRenderers) {
						hitHighlightMeshRenderer.material = brawlingMaterial;
					}

					if (!IsSeated) {
						PlayerComponent playerComponent = axe.GetComponentInParent<PlayerComponent>();
						Vector3 direction = (playerComponent.transform.position - transform.position).normalized;
						navMeshAgent.enabled = false;
						rb.isKinematic = false;
						rb.AddForce(direction * axe.WeaponData.knockBackStrength * -1, ForceMode.Impulse);
					}

					StartCoroutine(ResetHitHighLight());

					if(state is BrawlingVikingState) {
						Stats.TakeBrawlDamage(axe.WeaponData.brawlDamage);
						if(Stats.BrawlHealth <= 0) {

							ChangeState(new LeavingVikingState(this));
							return;
						}
					}

					if (state is WaitingForSeatVikingState) {
						ChangeState(new LeavingVikingState(this));
						return;
					}

					Stats.TakeMoodDamage(axe.WeaponData.moodDamage);

					if(Stats.Mood <= 15f) {
						ChangeState(new BrawlingVikingState(this, axe.GetComponentInParent<PlayerComponent>()));
					}
				}
			}
		}

		private IEnumerator ResetHitHighLight() {

			yield return new WaitForSeconds(0.5f);

			foreach (MeshRenderer hitHighlightMeshRenderer in hitHighlightMeshRenderers) {
				hitHighlightMeshRenderer.material = normalMaterial;
			}

			if (!IsSeated) {
				rb.isKinematic = true;
				navMeshAgent.enabled = true;
				state.Enter();
			}

			isAttacked = false;
		}
	}
}
