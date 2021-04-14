﻿using ScriptableObjects;
using UnityEngine;

namespace Vikings {
	public delegate void VikingLeaving(Viking sender);

	public class Viking : MonoBehaviour {
		[SerializeField] private VikingData vikingData;

		private VikingState state;
		private int desires;

		public VikingStats Stats { get; private set; }
		public event VikingLeaving LeaveTavern;

		private void Start() {
			state = new PassiveVikingState(this);

			Stats = new VikingStats(vikingData);
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

		public bool TryGiveItem() {
			// Returns true if the given item changes the state
			return state != (state = state.GiveItem());
		}

		public void Leave() {
			LeaveTavern?.Invoke(this);
		}
	}
}
