using ScriptableObjects;
using UnityEngine;

namespace Vikings {
	public class VikingStats {
		private VikingData data;

		private float mood;
		public float Mood => mood;

		public VikingStats(VikingData data) {
			this.data = data;
			mood = data.startMood;
		}

		public void Decline() {
			mood -= data.moodDeclineRate * Time.deltaTime;
		}

		public void Reset() {
			mood = data.startMood;
		}
	}
}
