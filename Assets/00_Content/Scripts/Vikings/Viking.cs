using System.Collections.Generic;
using Interactables;
using Rounds;
using UI;
using UnityEngine;
using Vikings.States;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public delegate void VikingLeavingQueue(Viking sender);

	public class Viking : Interactable {
		[SerializeField] private VikingData vikingData;
		[SerializeField] public GameObject beingSeatedHighlightPrefab;
		[SerializeField] public Coin coinPrefab;
		[SerializeField] public DesireVisualiser desireVisualiser;
		[SerializeField] public ProgressBar progressBar;
		[SerializeField] public MeshRenderer bodyMeshRenderer;
		[SerializeField] public Material normalMaterial;
		[SerializeField] public Material brawlingMaterial;

		private VikingState state;
		private VikingScaling statScaling;

		public VikingData Data => vikingData;
		public DesireData[] Desires => vikingData.desires;
		public DesireData CurrentDesire => vikingData.desires[CurrentDesireIndex];
		public List<float> MoodWhenDesireFulfilled { get; } = new List<float>();
		public VikingStats Stats { get; private set; }
		public Chair CurrentChair { get; set; }
		public int CurrentDesireIndex { get; set; }
		public int QueuePosition { get; set; }

		public event VikingLeaving LeaveTavern;
		public event VikingLeavingQueue LeaveQueue;

		private void Start() {
			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();

			ChangeState(new WaitingForSeatVikingState(this));
			Stats = new VikingStats(vikingData, statScaling);

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += HandleOnRoundOver;

			progressBar.Hide();
		}

		private void Update() {
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
	}
}
