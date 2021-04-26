using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Viking", menuName = "Game/Viking", order = 0)]
	public class VikingData : ScriptableObject {
		[Header("Mood")]
		[Range(0, 100)]
		public float startMood;

		[Tooltip("Mood/s")]
		public float moodDeclineRate;

		[Tooltip("Threshold for when the viking just takes a table on its own, without waiting for a player to lead them.")]
		public float impatientMoodThreshold = 45;

		[Header("Desire")]
		[Tooltip("What desires does this viking have")]
		public DesireData[] desires;

		[MinMaxRange(0, 100), Tooltip("How long does it take for the viking to start desiring something")]
		public Vector2 desireInterval;

		[Header("Combat")]
		[Tooltip("Will this viking be able to start a brawl")]
		public bool canStartBrawl = true;

		[EnableIf(nameof(canStartBrawl)), Tooltip("Threshold for when the viking will start brawling with other vikings")]
		public float brawlMoodThreshold = 15;

		[Tooltip("How much damage does the viking deal with a single attack")]
		public float damage = 10f;

		[Tooltip("How long does it take to perform one attack, s/attack")]
		public float attackRate = 1;
	}
}
