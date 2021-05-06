using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Cameras {
	public class FollowingCamera : MonoBehaviour {
		[SerializeField] private List<Transform> targets = new List<Transform>();
		[SerializeField] private Vector2 margins = new Vector2(1f, 1f);
		[SerializeField] private float minY = 10f;
		[SerializeField] private float smoothTime = 0.2f;
		[SerializeField] private float maxSpeed = 50f;

		private Vector3 initialPosition;
		private Vector3 currentVelocity;
		private Camera cam;

		private void Start() {
			cam = GetComponent<Camera>();
			initialPosition = transform.position;

			if (PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players)
					targets.Add(player.transform);

				PlayerManager.Instance.PlayerJoined += OnPlayerJoined;
				PlayerManager.Instance.PlayerLeft += OnPlayerLeft;
			}
		}

		private void FixedUpdate() {
			transform.position = Vector3.SmoothDamp(transform.position, CalculateTargetPosition(),
			                                        ref currentVelocity, smoothTime, maxSpeed);
		}

		private void OnDestroy() {
			if (PlayerManager.Instance != null) {
				PlayerManager.Instance.PlayerJoined -= OnPlayerJoined;
				PlayerManager.Instance.PlayerLeft -= OnPlayerLeft;
			}
		}

		private void OnPlayerJoined(PlayerComponent plr) {
			targets.Add(plr.transform);
		}

		private void OnPlayerLeft(PlayerComponent plr) {
			targets.Remove(plr.transform);
		}

		public void AddTarget(Transform newTarget) {
			targets.Add(newTarget);
		}

		public void RemoveTarget(Transform targetToRemove) {
			targets.Remove(targetToRemove);
		}

		// https://www.desmos.com/calculator/9gb8rpm4z1
		private Vector3 CalculateTargetPosition() {
			if (targets.Count == 0)
				return initialPosition;

			float ySlope = 1f / Mathf.Tan(cam.fieldOfView / 2f * Mathf.Deg2Rad);
			Vector2 frustumSlope = new Vector2(ySlope / cam.aspect, ySlope); // (z/x, z/y)

			Quaternion cameraRotation = transform.rotation;
			Quaternion inverseCameraRotation = Quaternion.Inverse(cameraRotation);

			Vector2 min = Vector2.positiveInfinity;
			Vector2 max = Vector2.negativeInfinity;

			foreach (Transform target in targets) {
				// Rotate into the camera's local space to make calculations easier
				Vector3 localPosition = inverseCameraRotation * target.transform.position;

				// X-axis intersection points of the margin adjusted projection lines
				Vector2 xIntersectOffsets = new Vector2(localPosition.z, localPosition.z) / frustumSlope - margins;
				Vector2 xIntersectLow = (Vector2)localPosition - xIntersectOffsets;
				Vector2 xIntersectHigh = (Vector2)localPosition + xIntersectOffsets;

				max = Vector2.Max(max, xIntersectLow);
				min = Vector2.Min(min, xIntersectHigh);
			}

			// Find the intersection point using the min-maxed x-axis intersections
			Vector2 halfDiff = (max - min) / 2f;
			Vector2 vertical = frustumSlope * halfDiff;
			Vector2 horizontal = min + halfDiff;

			Vector3 targetPoint = new Vector3(horizontal.x, horizontal.y, -Mathf.Max(vertical.x, vertical.y));

			// Rotate back into world space
			targetPoint = cameraRotation * targetPoint;

			// If we're below the height limit, move back up along the forward vector
			if (targetPoint.y < minY) {
				Vector3 forward = transform.forward;
				targetPoint += forward * (minY - targetPoint.y) / forward.y;
			}

			return targetPoint;
		}
	}
}
