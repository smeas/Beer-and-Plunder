using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "new Viking", menuName = "Game/VikingData", order = 0)]
	public class VikingData : ScriptableObject {
		[Range(0, 100)]
		public float startMood;

		[Tooltip("Mood/s")]
		public float moodDeclineRate;
	}
}
