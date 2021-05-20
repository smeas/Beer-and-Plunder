using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Interactables;
using Interactables.Kitchens;
using Interactables.Weapons;
using Player;
using Rounds;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Vikings.States;
using Random = UnityEngine.Random;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public delegate void VikingLeavingQueue(Viking sender);

	public class Viking : Interactable, IHittable {
		[SerializeField] private VikingData vikingData;
		[SerializeField] public GameObject beingSeatedHighlightPrefab;
		[SerializeField] public ParticleSystem disappearParticleSystem;
		[SerializeField] public float maxLeavingTime = 10f;
		[SerializeField] public Coin coinPrefab;
		[SerializeField] public KitchenTicket kitchenTicketPrefab;
		[SerializeField] public DesireVisualiser desireVisualiser;
		[SerializeField] public ProgressBar progressBar;
		[SerializeField] public Renderer bodyMeshRenderer;
		[SerializeField] public Material normalMaterial;
		[SerializeField] public Material brawlingMaterial;

		[Space]
		[SerializeField] public float itemThrowConeHalfAngle = 15f;
		[SerializeField] public float throwStrength = 5f;
		[SerializeField] public Transform pivotWhenSitting;
		[SerializeField] public Transform handTransform;

		private bool hasStartedAttackingPlayer;
		private VikingState state;
		private VikingState forcedState;
		private VikingScaling statScaling;
		private Rigidbody rb;
		private NavMeshAgent navMeshAgent;
		private bool isAttacked;
		private bool isAttacking;
		[NonSerialized] public Animator animator;
		[NonSerialized] public VikingAnimationDriver animationDriver;

		public NavMeshAgent NavMeshAgent => navMeshAgent;
		public VikingData Data => vikingData;
		public DesireData[] Desires { get; private set; }
		public DesireData CurrentDesire => Desires[CurrentDesireIndex];
		public List<float> MoodWhenDesireFulfilled { get; } = new List<float>();
		public VikingStats Stats { get; private set; }
		public Chair CurrentChair { get; set; }
		public int CurrentDesireIndex { get; set; }
		public int QueuePosition { get; set; }
		public bool IsAttacking => animationDriver.IsPlayingAttackAnimation;
		public bool IsAttacked { get => isAttacked; set => isAttacked = value; }

		public event VikingLeaving LeaveTavern;
		public event VikingLeavingQueue LeaveQueue;
		public Action TakingSeat;
		public Action BecameSatisfied;
		public Action Hit;

		private void Start() {
			animator = GetComponentInChildren<Animator>();
			animationDriver = GetComponentInChildren<VikingAnimationDriver>();

			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();
			rb = GetComponent<Rigidbody>();
			navMeshAgent = GetComponent<NavMeshAgent>();

			Stats = new VikingStats(vikingData, statScaling);

			ChangeState(new WaitingForSeatVikingState(this));

			progressBar.Hide();

			SetupDesires();
		}

		private void Update() {
			animationDriver.GettingAngry = Stats.Mood < Data.gettingAngryThreshold;

			if (Data.attackPlayerAtStartUp && !hasStartedAttackingPlayer) {
				PlayerComponent player = PlayerManager.Instance.Players.FirstOrDefault();

				if (player == null)
					return;

				ChangeState(new BrawlingVikingState(this, player));
				hasStartedAttackingPlayer = true;
			}

			if (forcedState == null) {
				ChangeState(state.Update());
			}
			else {
				ChangeState(forcedState);
				forcedState = null;
			}
		}

		private void SetupDesires() {
			if (!Data.randomDesires) {
				Desires = Data.desires;
				return;
			}

			int totalDesires = Random.Range(Mathf.RoundToInt(Data.desiresMinMax.x), Mathf.RoundToInt(Data.desiresMinMax.y + 1));
			Desires = MathX.RandomizeByWeight(Data.desires, x => x.randomWeight, totalDesires);
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

		public void ForceChangeState(VikingState newState) {
			forcedState = newState;
		}

		public void Leave() {
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

			animationDriver.EndSitting();
			CurrentChair.OnVikingLeaveChair(this);
			CurrentChair = null;
		}

		public void JoinBrawl() {
			ChangeState(new BrawlingVikingState(this, CurrentChair.Table));
		}

		public void MakeAttack() {
			if (!IsAttacking) {
				animationDriver.TriggerAttack();
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

		public void Affect(GameObject player, PickUp item) {
			state.Affect(player, item);
		}

		public void CancelAffect(GameObject player, PickUp item) {
			state.CancelAffect(player, item);
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return state.CanInteract(player, item);
		}

		public override void Interact(GameObject player, PickUp item) {
			ChangeState(state.Interact(player, item));
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			state.CancelInteraction(player, item);
		}

		void IHittable.Hit(Axe weapon) {
			RegisterHitFromPlayer(weapon);
		}

		private void RegisterHitFromPlayer(Axe axe) {

			if (state is LeavingVikingState)
				return;

			if (axe.IsAttacking && !isAttacked) {

				isAttacked = true;

				SetMaterial(brawlingMaterial);

				if (CurrentChair == null) {
					PlayerComponent playerComponent = axe.GetComponentInParent<PlayerComponent>();
					Vector3 direction = (playerComponent.transform.position - transform.position).normalized;
					navMeshAgent.enabled = false;
					rb.isKinematic = false;
					rb.AddForce(direction * axe.WeaponData.knockBackStrength * -1, ForceMode.Impulse);
				}

				StartCoroutine(ResetHitSimulation());
				VikingState vikingState = state.HandleOnHit(axe, this);
				Hit?.Invoke();
				ChangeState(vikingState);

				AudioManager.PlayEffectSafe(SoundEffect.Viking_AxeHit);
			}
		}

		private IEnumerator ResetHitSimulation() {

			yield return new WaitForSeconds(vikingData.iFrameAfterGettingHit);

			SetMaterial(normalMaterial);

			if (CurrentChair == null) {
				rb.isKinematic = true;
				navMeshAgent.Warp(rb.position);
				state.Enter();
			}

			isAttacked = false;
		}

		// TODO: Delete this
		public void SetMaterial(Material newMaterial) {
			int materialCount = bodyMeshRenderer.sharedMaterials.Length;
			bodyMeshRenderer.sharedMaterials = Enumerable.Repeat(newMaterial, materialCount).ToArray();
		}
	}
}
