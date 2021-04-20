using UnityEngine;

namespace Player {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMovement : MonoBehaviour {
		[SerializeField] private float walkingSpeed = 10f;
		[SerializeField] private bool canMove = true;

		private new Rigidbody rigidbody;
		private Camera mainCamera;

		private Vector2 moveInput;

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

			Vector3 movement = (forward * moveInput.y + right * moveInput.x) * (walkingSpeed * Time.deltaTime);
			rigidbody.MovePosition(transform.position + movement);
			transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
		}

		private (Vector3 forward, Vector3 right) GetCameraRelativeMovementDirections() {

			if(mainCamera == null)
				mainCamera = Camera.main;

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