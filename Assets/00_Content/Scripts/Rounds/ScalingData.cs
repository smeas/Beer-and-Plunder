using UnityEngine;

namespace Rounds {
	[CreateAssetMenu(fileName = "new Scaling", menuName = "Game/Scaling", order = 0)]
	public class ScalingData : ScriptableObject {
		[Header("SpawnDelay")]
		[SerializeField, Tooltip("Delay between viking spawns in seconds")]
		private AnimationCurve spawnDelayCurve;

		[SerializeField, Min(0), Tooltip("Excluded from the value in the Spawn Delay Curve")]
		public float spawnDelayVariance;

		[Header("StartingMood")]
		[SerializeField, Tooltip("Multiplier controlling how much mood vikings start to")]
		private AnimationCurve startingMoodMultiplierCurve;

		[Header("MoodDecline")]
		[SerializeField, Tooltip("Multiplier controlling how fast mood decline")]
		private AnimationCurve moodDeclineMultiplier;

		[Header("Other")]
		[Min(0)]
		public float tableHealth = 100f;

	#region ScalingCalculations

		/// <summary>
		/// Calculates the spawn delay in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledSpawnDelay(int round) {
			return EvalPointOnCurve(spawnDelayCurve, round - 1);
		}

		/// <summary>
		/// Calculates the starting mood in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledStartingMood(int round) {
			return EvalPointOnCurve(startingMoodMultiplierCurve, round - 1);
		}

		/// <summary>
		/// Calculates the mood decline multiplier in relation to <paramref name="round"/>
		/// </summary>
		public float ScaledMoodDeclineMultiplier(int round) {
			return EvalPointOnCurve(moodDeclineMultiplier, round - 1);
		}

		/// <summary>
		/// Calculates the value of a point clamped to a curve
		/// </summary>
		public static float EvalPointOnCurve(AnimationCurve curve, float point) {
			if (curve.length == 0) {
				Debug.LogError($"Curve does not have any keys");
				return 0;
			}

			return curve.Evaluate(point);
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
