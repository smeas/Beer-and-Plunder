using System.Linq;
using UnityEngine;
using Utilities;

namespace Vikings.States {
	public class SatisfiedVikingState : VikingState {
		private const float DropDelay = 0.15f;

		private float satisfiedDuration;
		private float satisfiedTimer;

		private int coinsToDrop;
		private float dropTimer;

		private DesireType previousDesire;

		private bool IsDroppingCoins => coinsToDrop > 0;

		public SatisfiedVikingState(Viking viking, DesireType previousDesire) : base(viking) {
			this.previousDesire = previousDesire;
		}

		public override VikingState Enter() {
			satisfiedDuration =
				Random.Range(viking.Data.satisfiedDurationMinMax.x, viking.Data.satisfiedDurationMinMax.y);
			satisfiedTimer = satisfiedDuration;

			return this;
		}

		public override void Exit() {
			if (previousDesire.IsBeer()) {
				Rigidbody tankardBody = Object.Instantiate(viking.beerPrefab,
				                                           viking.transform.position + new Vector3(0, 2.5f, 0),
				                                           Quaternion.identity).GetComponent<Rigidbody>();

				Vector3 throwDirection = -viking.transform.forward;
				throwDirection.y = 0.7f;

				tankardBody.velocity = MathX.RandomDirectionInCone(throwDirection, viking.tankardThrowConeHalfAngle) *
					viking.tankardThrowStrength;
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
			return Mathf.RoundToInt(MathX.Remap(value, viking.Data.brawlMoodThreshold,
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
