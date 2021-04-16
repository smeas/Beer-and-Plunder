using Rounds;
using UnityEngine;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public class Viking : MonoBehaviour {
		[SerializeField] private VikingData vikingData;

		private VikingState state;
		private VikingScaling statScaling;
		private int desires;

		public VikingStats Stats { get; private set; }
		public event VikingLeaving LeaveTavern;

		private void Start() {
			// statScaling is normally provided by the viking manager
			statScaling ??= new VikingScaling();

			state = new PassiveVikingState(this);
			Stats = new VikingStats(vikingData, statScaling);
			desires = 2;
		}

		private void Update() {
			state = state.Update();

			if (desires > 0 && Stats.Mood < 25) {
				if (TryGiveItem()) {
					desires--;
					Stats.Reset();
				}
			}
			
			if (desires <= 0)
				Leave();
		}

		public void SetScaling(VikingScaling scaling) {
			statScaling = scaling;
		}

		public bool TryGiveItem() {
			// Returns true if the given item changes the state
			return state != (state = state.GiveItem());
		}

		public void Leave() {
			LeaveTavern?.Invoke(this);
		}
	}
}
