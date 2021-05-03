using UnityEngine;

namespace Player {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMovement : MonoBehaviour {
		[SerializeField] private float walkingSpeed = 10f;
		[SerializeField] private float speedMultiplier = 1f;
		[SerializeField] private bool canMove = true;

		private new Rigidbody rigidbody;
		private Camera mainCamera;

		private Vector2 moveInput;

		public float SpeedMultiplier {
			get => speedMultiplier;
			set => speedMultiplier = value;
		}

		/// <summary>
		/// Should the player be allowed to move?
		/// </summary>
		public bool CanMove {
			get => canMove;
			set => canMove = value;
		}

		private void Start() {
			rigidbody = GetComponent<Rigidbody>();
			mainCamera = Camera.main;
		}

		private void FixedUpdate() {
			if (!canMove || moveInput == Vector2.zero)
				return;

			(Vector3 forward, Vector3 right) = GetCameraRelativeMovementDirections();

			Vector3 movement = (forward * moveInput.y + right * moveInput.x) * (walkingSpeed * speedMultiplier * Time.deltaTime);
			if (movement == Vector3.zero)
				return;

			rigidbody.MovePosition(transform.position + movement);
			transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
		}

		private (Vector3 forward, Vector3 right) GetCameraRelativeMovementDirections() {
			// This is needed because the main camera changes between scenes.
			if (mainCamera == null) {
				mainCamera = Camera.main;

				if (mainCamera == null) {
					Debug.LogError("No main camera found", this);
					return (Vector3.forward, Vector3.right);
				}
			}

			Transform cameraTransform = mainCamera.transform;

			Vector3 forward = cameraTransform.forward;

			// If the camera is looking straight up/down, use it's up vector instead.
			if (Mathf.Abs(forward.y) > 0.999f)
				forward = cameraTransform.up;

			// Project the camera's forward vector onto the XZ (ground) plane to get the forward direction.
			forward.y = 0;
			forward.Normalize();

			return (forward, cameraTransform.right);
		}

		public void Move(Vector2 input) {
			moveInput = input;
		}
	}
}