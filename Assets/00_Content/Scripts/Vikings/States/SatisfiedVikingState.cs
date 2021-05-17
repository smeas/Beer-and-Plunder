using System.Linq;
using Interactables;
using Interactables.Beers;
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
		private DesireData satisfiedDesire;

		private bool IsDroppingCoins => coinsToDrop > 0;

		public SatisfiedVikingState(Viking viking, DesireData satisfiedDesire) : base(viking) {
			this.satisfiedDesire = satisfiedDesire;
		}

		public SatisfiedVikingState(Viking viking, DesireData satisfiedDesire, PickUp givenItem) : this(viking, satisfiedDesire) {
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

				if (satisfiedDesire.shouldThrowItem) {
					givenItem.gameObject.SetActive(true);
					givenItem.transform.position = viking.transform.position + new Vector3(0, 2.5f, 0);

					Vector3 throwDirection = -viking.transform.forward;
					throwDirection.y = 0.7f;

          givenItem.GetComponent<Rigidbody>().velocity = MathX.RandomDirectionInCone(throwDirection, viking.itemThrowConeHalfAngle) * viking.throwStrength;
				}
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

				if (coinsToDrop <= 0)
					return SelectNextState();

				dropTimer = DropDelay;
			}

			return this;
		}

		private void DropCoin() {
			Coin coin = Object.Instantiate(viking.coinPrefab, viking.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
			coin.ThrowInDirection(MathX.RandomDirectionInQuarterSphere(-viking.transform.forward, Vector3.up));
			coinsToDrop--;
		}

		private VikingState SelectNextState() {
			if (viking.CurrentDesireIndex >= viking.Desires.Length)
				return new LeavingVikingState(viking);

			return new PassiveVikingState(viking);
		}
	}
}
