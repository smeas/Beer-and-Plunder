using Interactables;
using Rounds;
using UnityEngine;
using Vikings.States;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public delegate void VikingLeavingQueue(Viking sender);

	public class Viking : Interactable {
		[SerializeField] private VikingData vikingData;
		[SerializeField] public GameObject beingSeatedHighlightPrefab;
		[SerializeField] public MeshRenderer bodyMeshRenderer;
		[SerializeField] public Material normalMaterial;
		[SerializeField] public Material desiringMaterial;

		private VikingState state;
		private VikingScaling statScaling;

		public VikingData Data => vikingData;
		public VikingStats Stats { get; private set; }
		public Chair CurrentChair { get; set; }
		public int Desires { get; set; }
		public int QueuePosition { get; set; }

		public event VikingLeaving LeaveTavern;
		public event VikingLeavingQueue LeaveQueue;

		private void Start() {
			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();

			ChangeState(new WaitingForSeatVikingState(this));
			Stats = new VikingStats(vikingData, statScaling);
			Desires = 2;

			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += HandleOnRoundOver;
		}

		private void Update() {
			ChangeState(state.Update());
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
	}
}
