using UnityEngine;

namespace Utilities {
	public static class MathX {
		public static float EaseInQuad(float x) {
			return x * x;
		}

		public static float EaseOutQuad(float x) {
			return 1 - (1 - x) * (1 - x);
		}

		public static float EaseInOutQuad(float x) {
			return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
		}

		public static float Remap(float value, float low1, float high1, float low2, float high2) {
			return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
		}

		/// <summary>
		/// Get a random direction within a cone.
		/// </summary>
		/// <param name="direction">The direction of the cone.</param>
		/// <param name="halfAngle">Half the angle of the cone measured in radians.</param>
		public static Vector3 RandomDirectionInCone(Vector3 direction, float halfAngle) {
			float radius = Mathf.Tan(halfAngle);
			Vector2 pointInCircle = Random.insideUnitCircle * radius;
			return (Quaternion.LookRotation(direction) * new Vector3(pointInCircle.x, pointInCircle.y, 1f)).normalized;
		}
	}
}
