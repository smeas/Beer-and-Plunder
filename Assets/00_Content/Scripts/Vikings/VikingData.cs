using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Viking", menuName = "Game/Viking", order = 0)]
	public class VikingData : ScriptableObject {
		[Header("Mood")]
		[Range(0, 100)]
		public float startMood;

		[Tooltip("Mood/s")]
		public float moodDeclineRate;

		[Min(0), Tooltip("How much mood does a viking receive when a desire is fulfilled")]
		public float moodBoostDesireFulfilled;

		[Tooltip("Threshold for when the viking just takes a table on its own, without waiting for a player to lead them.")]
		public float impatientMoodThreshold = 45;

		[Tooltip("Threshold for when the viking starts to show visible anger")]
		public float gettingAngryThreshold = 30f;

		[Header("Desire")]
		public Sprite desireSeatVisualisation;

		public bool randomDesires = true;

		[Tooltip("What desires can this viking have")]
		public DesireData[] desires;

		[MinMaxRange(1, 10), Tooltip("How many desires does this viking have (Rounded to whole numbers)")]
		public Vector2 desiresMinMax;

		[MinMaxRange(0, 100), Tooltip("How long does it take for the viking to start desiring something")]
		public Vector2 desireIntervalMinMax;


		[Space]
		[MinMaxRange(0, 100), Tooltip("How long does the viking stay satisfied upon fulfilling a desire")]
		public Vector2 satisfiedDurationMinMax;

		[MinMaxRange(0, 100), Tooltip("How many coins does the viking drop when a desire is fulfilled")]
		public Vector2 coinsDroppedMinMax;

		[Min(0), Tooltip("How many coins does the viking drop when all desires are fulfilled")]
		public float coinsWhenLeavingMultiplier = 1f;

		public bool FollowPlayerIndefinitely = false;

		[Min(0)]
		public float MaxFollowingDuration = 10f;


		[Header("Combat")]
		[Tooltip("Will this viking be able to start a brawl")]
		public bool canStartBrawl = true;

		[EnableIf(nameof(canStartBrawl)), Tooltip("Threshold for when the viking will start brawling with other vikings")]
		public float brawlMoodThreshold = 15;

		[Tooltip("How much damage does the viking deal with a single attack")]
		public float damage = 1f;

		[Tooltip("How long does it take to perform one attack, s/attack")]
		public float attackRate = 1;

		[Tooltip("How many hits can be taken when brawling")]
		public float brawlHealth = 3;

		[Tooltip("How far the viking is from the player before starting to attack")]
		public float attackTriggerDistance = 4f;

		[Tooltip("The time it takes for the rigidbody to take in collisions again after getting hit")]
		public float iFrameAfterGettingHit = 0.5f;


		[Header("Debug")]
		[Tooltip("Make the vikings attack the player as a default behaviour (for debugging purpose)")]
		public bool attackPlayerAtStartUp;
	}
}
