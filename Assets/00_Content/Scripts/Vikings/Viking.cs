using Interactables;
using Rounds;
using UnityEngine;
using Vikings.States;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public class Viking : Interactable {
		[SerializeField] private VikingData vikingData;
		[SerializeField] public GameObject beingSeatedHighlightPrefab;
		[SerializeField] public MeshRenderer bodyMeshRenderer;
		[SerializeField] public Material normalMaterial;
		[SerializeField] public Material desiringMaterial;

		private VikingState state;
		private VikingScaling statScaling;
		private int desires;

		public VikingStats Stats { get; private set; }
		public Chair CurrentChair { get; set; }

		public event VikingLeaving LeaveTavern;

		private void Start() {
			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();

			ChangeState(new WaitingForSeatVikingState(this));
			Stats = new VikingStats(vikingData, statScaling);
			desires = 2;
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

		public void SetScaling(VikingScaling scaling) {
			statScaling = scaling;
		}

		public bool TryTakeSeat(Chair chair) {
			return ChangeState(state.TakeSeat(chair));
		}

		public void Leave() {
			// Maybe we should have a leaving state here instead?
			LeaveTavern?.Invoke(this);

			if (CurrentChair != null) {
				CurrentChair.OnVikingLeaveChair(this);
				CurrentChair = null;
			}
		}

		public override void Interact(GameObject player, PickUp item) {
			ChangeState(state.Interact(player, item));
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return state.CanInteract(player, item);
		}
	}
}
