using Rounds;
using UnityEngine;

namespace Vikings {
	public class VikingStats {
		private float startMood;

		private float mood;
		public float Mood => mood;

		private float moodDeclineRate;

		public VikingStats(VikingData data, VikingScaling scaling) {
			startMood = data.startMood * scaling.startMoodMultiplier;
			mood = startMood;
			moodDeclineRate = data.moodDeclineRate * scaling.moodDeclineMultiplier;
		}

		public void Decline() {
			mood -= moodDeclineRate * Time.deltaTime;
		}

		public void Reset() {
			mood = startMood;
		}
	}
}
