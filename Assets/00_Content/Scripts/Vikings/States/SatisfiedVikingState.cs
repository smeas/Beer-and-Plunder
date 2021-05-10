using System.Linq;
using Interactables;
using Interactables.Beers;
using Rounds;
using UnityEngine;
using Utilities;

namespace Vikings.States {
	public class SatisfiedVikingState : VikingState {
		private const float DropDelay = 0.15f;

		private float satisfiedDuration;
		private float satisfiedTimer;

		private int coinsToDrop;
		private float dropTimer;

		private PickUp givenItem;

		private bool IsDroppingCoins => coinsToDrop > 0;

		public SatisfiedVikingState(Viking viking) : base(viking) { }

		public SatisfiedVikingState(Viking viking, PickUp givenItem) : this(viking) {
			this.givenItem = givenItem;
		}

		public override VikingState Enter() {
			satisfiedDuration =
				Random.Range(viking.Data.satisfiedDurationMinMax.x, viking.Data.satisfiedDurationMinMax.y);
			satisfiedTimer = satisfiedDuration;

			viking.BecameSatisfied?.Invoke();

			return this;
		}

		public override void Exit() {
			if (givenItem != null) {
				if (givenItem is Tankard tankard)
					tankard.IsFull = false;

				givenItem.gameObject.SetActive(true);
				givenItem.transform.position = viking.transform.position + new Vector3(0, 2.5f, 0);

				Vector3 throwDirection = -viking.transform.forward;
				throwDirection.y = 0.7f;

				//if (RoundController.Instance != null && !RoundController.Instance.IsRoundActive)
				//	givenItem.Respawn();
				//else
					givenItem.GetComponent<Rigidbody>().velocity = MathX.RandomDirectionInCone(throwDirection, viking.tankardThrowConeHalfAngle) * viking.tankardThrowStrength;
			}
		}

		public override VikingState Update() {
			VikingState nextState = this;
			satisfiedTimer -= Time.deltaTime;

			if (satisfiedTimer <= 0 && !IsDroppingCoins)
				SetupDroppingCoins();

			if (IsDroppingCoins)
				nextState = DropCoins();

			return nextState;
		}

		private void SetupDroppingCoins() {
			if (viking.CurrentDesireIndex < viking.Desires.Length) {
				coinsToDrop = CalculateCoinsToDrop(viking.Stats.Mood);
			}
			else {
				float avgMood = viking.MoodWhenDesireFulfilled.Sum(x => x) /
				              viking.MoodWhenDesireFulfilled.Count;

				coinsToDrop = CalculateCoinsToDrop(avgMood);
				coinsToDrop = Mathf.RoundToInt(coinsToDrop * viking.Data.coinsWhenLeavingMultiplier);
			}

			dropTimer = DropDelay;
		}

		private int CalculateCoinsToDrop(float value) {
			return Mathf.RoundToInt(MathX.RemapClamped(value, viking.Data.brawlMoodThreshold,
				viking.Stats.StartMood, viking.Data.coinsDroppedMinMax.x, viking.Data.coinsDroppedMinMax.y));
		}

		private VikingState DropCoins() {
			dropTimer -= Time.deltaTime;

			if (dropTimer <= 0) {
				DropCoin();

				if (coinsToDrop <= 0) {
					viking.Stats.Reset();
					return SelectNextState();
				}

				dropTimer = DropDelay;
			}

			return this;
		}

		private void DropCoin() {
			Object.Instantiate(viking.coinPrefab, viking.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

			coinsToDrop--;
		}

		private VikingState SelectNextState() {
			if (viking.CurrentDesireIndex >= viking.Desires.Length)
				return new LeavingVikingState(viking);

			return new PassiveVikingState(viking);
		}
	}
}
