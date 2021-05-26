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

		private bool isWaitingForHappyToEnd;
		private bool isDroppingCoins;
		private bool isThrowing;
		private bool hasThrownItem;
		private bool hasHappyEnded;
		public bool isExitingBecauseOfLeaving;

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

			if (satisfiedDesire.type == DesireType.Food)
				viking.animationDriver.Eating = true;
			else if (satisfiedDesire.type == DesireType.Beer)
				viking.animationDriver.Drinking = true;

			return this;
		}

		public override void Exit() {
			viking.animationDriver.Eating = false;
			viking.animationDriver.Drinking = false;

			if (givenItem != null) {
				if (satisfiedDesire.shouldThrowItem) {
					if (!hasThrownItem) {
						// If the state is force changed, just do the old teleport->throw
						BeginThrowItem();
						givenItem.transform.position = viking.transform.position + new Vector3(0, 2.5f, 0);
						EndThrowItem();
					}
				}
				else {
					Object.Destroy(givenItem.gameObject);
				}
			}

			// TODO: Prettify throwing of coins
			if (satisfiedTimer > 0 && isExitingBecauseOfLeaving) {
				SetupDroppingCoins();

				while (coinsToDrop > 0)
					DropCoin();
			}

			viking.SatisfiedEnd?.Invoke();
		}

		public override VikingState Update() {
			satisfiedTimer -= Time.deltaTime;

			if (satisfiedTimer <= 0 && !isDroppingCoins) {
				viking.animationDriver.Eating = false;
				viking.animationDriver.Drinking = false;
				viking.animationDriver.TriggerHappy();
				isWaitingForHappyToEnd = true;

				if (givenItem is Tankard tankard)
					tankard.IsFull = false;

				if (satisfiedDesire.isMaterialDesire && !satisfiedDesire.shouldThrowItem)
					Object.Destroy(givenItem.gameObject);

				SetupDroppingCoins();
			}

			if (isThrowing && !viking.animationDriver.IsThrowing) {
				isThrowing = false;
				EndThrowItem();
			}

			if (isWaitingForHappyToEnd && !viking.animationDriver.IsPlayingHappyAnimation) {
				isWaitingForHappyToEnd = false;
				hasHappyEnded = true;
			}

			if (satisfiedDesire.shouldThrowItem && !isThrowing && !hasThrownItem && hasHappyEnded) {
				BeginThrowItem();
			}

			if (isDroppingCoins) {
				if (coinsToDrop > 0) {
					DropCoins();
				}
				else {
					if (!hasHappyEnded) return this;
					if (satisfiedDesire.shouldThrowItem && !hasThrownItem) return this;

					return SelectNextState();
				}
			}

			return this;
		}

		private void BeginThrowItem() {
			viking.animationDriver.TriggerThrow();
			isThrowing = true;
		}

		private void EndThrowItem() {
			hasThrownItem = true;
			givenItem.VikingDropItem();

			Vector3 throwDirection = -viking.transform.forward;
			throwDirection.y = 0.7f;

			givenItem.GetComponent<Rigidbody>().velocity =
				MathX.RandomDirectionInCone(throwDirection, viking.itemThrowConeHalfAngle) *
				viking.throwStrength;
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
			isDroppingCoins = true;
		}

		private int CalculateCoinsToDrop(float value) {
			return Mathf.RoundToInt(MathX.RemapClamped(value, viking.Data.brawlMoodThreshold,
				viking.Stats.StartMood, viking.Data.coinsDroppedMinMax.x, viking.Data.coinsDroppedMinMax.y));
		}

		private void DropCoins() {
			dropTimer -= Time.deltaTime;

			if (dropTimer <= 0) {
				DropCoin();

				if (coinsToDrop <= 0)
					return;

				dropTimer = DropDelay;
			}
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
