using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "new Viking", menuName = "VikingData", order = 0)]
	public class VikingData : ScriptableObject {
		[Tooltip("Mood/s")]
		public float moodDeclineRate;

		public float startMood;
	}
}
