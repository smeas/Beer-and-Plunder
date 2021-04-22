using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Viking", menuName = "Game/VikingData", order = 0)]
	public class VikingData : ScriptableObject {
		[Range(0, 100)]
		public float startMood;

		[Tooltip("Mood/s")]
		public float moodDeclineRate;

		public float desiringMoodThreshold = 50;

		[Tooltip("Threshold for when the viking just takes a table on its own, without waiting for a player to lead them.")]
		public float impatientMoodThreshold = 45;

		[Tooltip("What desires does this viking have")]
		public DesireData[] desires;
	}
}
