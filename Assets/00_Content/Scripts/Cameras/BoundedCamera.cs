using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Cameras {
	public class BoundedCamera : MonoBehaviour {
		[SerializeField] private List<Transform> targets = new List<Transform>();
		[SerializeField] private Vector2 margins = new Vector2(1f, 1f);
		[SerializeField] private float minY = 10f;
		[SerializeField] private float smoothTime = 0.2f, maxSpeed = 50f;
		[SerializeField] private bool showGizmos;

		private Vector3 initialPosition;
		private Vector3 currentVelocity;
		private Camera cam;

		private void Start() {
			cam = GetComponent<Camera>();

			initialPosition = transform.position;

			if (PlayerManager.Instance != null) {
				foreach (PlayerComponent player in PlayerManager.Instance.Players) {
					targets.Add(player.transform);
				}

				PlayerManager.Instance.PlayerJoined += OnPlayerJoined;
				PlayerManager.Instance.PlayerLeft += OnPlayerLeft;
			}
		}

		private void FixedUpdate() {
			Vector3 targetPosition = CalculateTargetPosition();
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity,
			                                        smoothTime, maxSpeed, Time.fixedDeltaTime);
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

		private Vector3 CalculateTargetPosition() {
			if (targets.Count == 0)
				return initialPosition;

			// Slope of frustum planes
			// 1 / tan flips the angle across the 45° diagonal (or tan(90 - x))
			float ySlope = 1f / Mathf.Tan(cam.fieldOfView * (Mathf.Deg2Rad * 0.5f));
			Vector2 slope = new Vector2(ySlope / cam.aspect, ySlope);

			Vector2 min = Vector2.positiveInfinity, max = Vector2.negativeInfinity;

			Matrix4x4 cameraToWorldRotation = new Matrix4x4(transform.right, transform.up, transform.forward, Vector3.zero);
			Matrix4x4 worldToCameraRotation = cameraToWorldRotation.transpose;

			foreach (Transform target in targets) {
				Vector3 localPosition = worldToCameraRotation * target.position; // derotate around origin
				Vector2 j = new Vector2(localPosition.z, localPosition.z) / slope - margins;
				max = Vector2.Max(max, (Vector2)localPosition - j);
				min = Vector2.Min(min, (Vector2)localPosition + j);
			}

			Vector2 diff = (max - min) * 0.5f;
			Vector2 h = diff * slope;
			Vector2 v = min + diff;

			Vector3 pos = cameraToWorldRotation * new Vector3(v.x, v.y, -Mathf.Max(h.x, h.y));
			if (pos.y < minY)
				pos += transform.forward * (minY - pos.y) / transform.forward.y;

			return pos;
		}

		private void OnDrawGizmos() {
			if (!showGizmos) return;

			Matrix4x4 m = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;

			foreach (Transform t in targets) {
				Gizmos.DrawWireCube(transform.InverseTransformPoint(t.position),
				                    new Vector3(margins.x * 2, margins.y * 2, 0.1f));
			}

			Gizmos.matrix = m;
		}
	}
}