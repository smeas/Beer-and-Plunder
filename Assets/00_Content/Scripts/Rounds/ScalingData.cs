using UnityEngine;

namespace Rounds {
	[CreateAssetMenu(fileName = "new ScalingData", menuName = "Game/ScalingData", order = 0)]
	public class ScalingData : ScriptableObject {
		[SerializeField, Min(0), Header("SpawnDelay")]
		private float initialSpawnDelay = 1f;

		[SerializeField, Range(0, 1), Tooltip("A lower value decreases the time to get to minimum")]
		private float spawnDelayRate = 0.5f;

		[SerializeField, Min(0), Tooltip("Should be lower than InitialSpawnDelay")]
		private float minimumSpawnDelay = 0f;

		[SerializeField, Min(0), Tooltip("Excluded from initial and minimum spawnDelay")]
		public float spawnDelayVariance;

		[SerializeField, Min(0), Header("StartingMood")]
		private float initialStartingMoodMultiplier = 1f;

		[SerializeField, Range(0, 1), Tooltip("A lower value decreases the time to get to minimum")]
		private float startingMoodMultiplierRate = 0.5f;

		[SerializeField, Min(0), Tooltip("Should be lower than InitialStartingMoodMultiplier")]
		private float minimumStartingMoodMultiplier = 0f;

		[SerializeField, Min(0), Header("MoodDecline")]
		private float initialMoodDeclineMultiplier = 1f;

		[SerializeField, Min(1), Tooltip("A higher value decreases the time to get to maximum")]
		private float moodDeclineMultiplierRate = 1.1f;

		[SerializeField, Min(0), Tooltip("Should be higher than InitialMoodDeclineMultiplier")]
		private float maximumMoodDeclineMultiplier = 2f;

		[Min(0)]
		public float tableHealth = 100f;

		#region ScalingCalculations

		/// <summary>
		/// Scales the spawn delay exponentially in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledSpawnDelay(int round) {
			return CalcExponential(initialSpawnDelay - minimumSpawnDelay, spawnDelayRate,
				round - 1, minimumSpawnDelay);
		}

		/// <summary>
		/// Scales the starting mood linearly in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledStartingMood(int round) {
			return CalcExponential(initialStartingMoodMultiplier - minimumStartingMoodMultiplier,
				startingMoodMultiplierRate, round - 1, minimumStartingMoodMultiplier);
		}

		/// <summary>
		/// Scales the mood decline multiplier linearly in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledMoodDeclineMultiplier(int round) {
			return Mathf.Clamp(
				CalcExponential(initialMoodDeclineMultiplier, moodDeclineMultiplierRate, round - 1, 0),
				initialMoodDeclineMultiplier, maximumMoodDeclineMultiplier);
		}

		/// <summary>
		/// Calculates a value following f(x) = kx + m
		/// </summary>
		public static float CalcLinear(float k, int x, float m) {
			return k * x + m;
		}

		/// <summary>
		/// Calculates a value following f(x) = ab^x + c
		/// </summary>
		public static float CalcExponential(float a, float b, float x, float c) {
			return a * Mathf.Pow(b, x) + c;
		}

		#endregion
	}
}
