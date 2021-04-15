using ScriptableObjects;

namespace Rounds {
	public class VikingScaling {
		public readonly float startMoodMultiplier = 1f;
		public readonly float moodDeclineMultiplier = 1f;

		public VikingScaling() {
		}

		public VikingScaling(ScalingData data, int round) {
			this.startMoodMultiplier = data.ScaledStartingMood(round);
			this.moodDeclineMultiplier = data.ScaledMoodDeclineMultiplier(round);
		}
	}
}
