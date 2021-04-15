using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "new ScalingData", menuName = "ScalingData", order = 0)]
	public class ScalingData : ScriptableObject {
		[field: SerializeField] private float InitialSpawnDelay { get; set; } = 1f;

		[field: SerializeField, Tooltip("Additive change/round")]
		private float SpawnDelayMultiplier { get; set; } = 1f;

		[field: SerializeField, Tooltip("Additive change/round")]
		private float StartingMoodMultiplier { get; set; } = 1f;

		[field: SerializeField, Tooltip("Additive change/round")]
		private float MoodDeclineMultiplier { get; set; } = 1f;

		// These calculations will need to be modified later to balance the game
	#region ScalingCalculations

		/// <summary>
		/// Scales the spawn delay linearly in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledSpawnDelay(int round) {
			return InitialSpawnDelay * CalcLinear(-SpawnDelayMultiplier + 1, round, 1);
		}

		/// <summary>
		/// Scales the starting mood linearly in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledStartingMood(int round) {
			return CalcLinear(StartingMoodMultiplier - 1, round, 1);
		}

		/// <summary>
		/// Scales the mood decline multiplier linearly in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledMoodDeclineMultiplier(int round) {
			return CalcLinear(MoodDeclineMultiplier - 1, round, 1);
		}

		/// <summary>
		/// Calculates a value following f(x) = kx + m
		/// </summary>
		public static float CalcLinear(float k, int x, float m) {
			return k * x + m;
		}

		#endregion

	}
}
