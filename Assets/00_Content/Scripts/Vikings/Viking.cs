using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Extensions;
using Interactables;
using Interactables.Beers;
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

	public class Viking : Interactable {
		[SerializeField] private VikingData vikingData;
		[SerializeField] private LayerMask weaponLayer;
		[SerializeField] public GameObject beingSeatedHighlightPrefab;
		[SerializeField] public Coin coinPrefab;
		[SerializeField] public Tankard tankardPrefab;
		[SerializeField] public DesireVisualiser desireVisualiser;
		[SerializeField] public ProgressBar progressBar;
		[SerializeField] public MeshRenderer bodyMeshRenderer;
		[SerializeField] public Material normalMaterial;
		[SerializeField] public Material brawlingMaterial;

		[Space]
		[SerializeField] public float tankardThrowConeHalfAngle = 15f;
		[SerializeField] public float tankardThrowStrength = 5f;

		private bool hasStartedAttackingPlayer;
		private VikingState state;
		private VikingState forcedState;
		private VikingScaling statScaling;
		private Rigidbody rb;
		private NavMeshAgent navMeshAgent;
		private bool isAttacked;
		private bool isAttacking;

		public VikingData Data => vikingData;
		public DesireData[] Desires { get; private set; }
		public DesireData CurrentDesire => Desires[CurrentDesireIndex];
		public List<float> MoodWhenDesireFulfilled { get; } = new List<float>();
		public VikingStats Stats { get; private set; }
		public Chair CurrentChair { get; set; }
		public int CurrentDesireIndex { get; set; }
		public int QueuePosition { get; set; }
		public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
		public bool IsAttacked { get => isAttacked; set => isAttacked = value; }

		public event VikingLeaving LeaveTavern;
		public event VikingLeavingQueue LeaveQueue;
		public Action TakingSeat;
		public Action BecameSatisfied;
		public Action Hit;

		private void Start() {
			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();
			rb = GetComponent<Rigidbody>();
			navMeshAgent = GetComponent<NavMeshAgent>();

			Stats = new VikingStats(vikingData, statScaling);

			ChangeState(new WaitingForSeatVikingState(this));

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += HandleOnRoundOver;

			progressBar.Hide();

			SetupDesires();
		}

		private void Update() {

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

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= HandleOnRoundOver;
		}

		private void SetupDesires() {
			if (!Data.randomDesires) {
				Desires = Data.desires;
				return;
			}

			int totalDesires = Random.Range(Mathf.RoundToInt(Data.desiresMinMax.x), Mathf.RoundToInt(Data.desiresMinMax.y + 1));
			Desires = MathX.RandomizeByWeight(Data.desires, x => x.weight, totalDesires);
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
			if (!IsAttacking) {
				IsAttacking = true;
				StartCoroutine(SpinAttack());
				StartCoroutine(FinishAttack());
			}
		}

		private IEnumerator SpinAttack() {
			while(IsAttacking) {
				yield return null;
				transform.Rotate(Vector3.up, vikingData.spinAttackSpeed * Time.deltaTime);
			}
		}

		private IEnumerator FinishAttack() {

			yield return new WaitForSeconds(vikingData.spinAttackDuration);

			StopAllCoroutines();
			IsAttacking = false;
			IsAttacked = false;
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

		private void OnTriggerEnter(Collider other) {
			if (weaponLayer.ContainsLayer(other.gameObject.layer)) {
				RegisterHitFromPlayer(other);
			}
		}

		private void RegisterHitFromPlayer(Collider other) {

			if (state is LeavingVikingState)
				return;

			//TODO - Probably get some more generic type of weapon instead of axe. Make an interface for all weapons?
			Axe axe = other.gameObject.GetComponentInParent<Axe>();

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
