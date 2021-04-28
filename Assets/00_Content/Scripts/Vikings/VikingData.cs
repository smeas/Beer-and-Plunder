using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Viking", menuName = "Game/VikingData", order = 0)]
	public class VikingData : ScriptableObject {
		[Range(0, 100)]
		public float startMood;

		[Tooltip("Mood/s")]
		public float moodDeclineRate;

		[Tooltip("Threshold for when the viking will start desiring something")]
		public float desiringMoodThreshold = 50;

		[Tooltip("Threshold for when the viking just takes a table on its own, without waiting for a player to lead them.")]
		public float impatientMoodThreshold = 45;

		[Tooltip("What desires does this viking have")]
		public DesireData[] desires;

		[Tooltip("Will this viking be able to start a brawl")]
		public bool canStartBrawl = true;

		[EnableIf(nameof(canStartBrawl)), Tooltip("Threshold for when the viking will start brawling with other vikings")]
		public float brawlMoodThreshold = 15;

		[Tooltip("How much damage does the viking deal with a single attack")]
		public float damage = 10f;

		[Tooltip("How long does it take to perform one attack, s/attack")]
		public float attackRate = 1;

		[Tooltip("How many hits can be taken when brawling")]
		public float brawlHealth = 3;

		public float attackTriggerDistance = 4f;

		public float spinAttackSpeed = 8f;

		public float spinAttackDuration = 2f;

		public float attackDamage = 1f;


		[Header("Debug")]
		public bool defaultAttackPlayer;
	}
}
