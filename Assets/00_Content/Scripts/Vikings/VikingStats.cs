using UnityEngine;

namespace Vikings {
	public class VikingStats {
		private float moodDeclineRate = 10;

		private float mood;
		public float Mood => mood;

		public VikingStats() {
			mood = 100;
		}

		public void Decline() {
			mood -= moodDeclineRate * Time.deltaTime;
		}

		public void Reset() {
			mood = 100;
		}
	}
}
