using UnityEngine;

namespace Player {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMovement : MonoBehaviour {
		[SerializeField] private float acceleration = 48f;
		[SerializeField] private float deceleration = 48f;
		[SerializeField] private float maxVelocity = 6f;
		[SerializeField] private float speedMultiplier = 1f;
		[SerializeField] private bool canMove = true;
		[SerializeField] private bool canRotate = true;

		private new Rigidbody rigidbody;
		private Camera mainCamera;

		private Vector2 moveInput;
		private Vector2 movementDirection;
		private Vector3 movement;
		private float velocity;

		public float Velocity => velocity;
		public Vector2 MoveInput => moveInput;

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

		public bool CanRotate {
			get => canRotate;
			set => canRotate = value;
		}
		public bool CalculateMovementDirection { get; set; } = true;

		private void Start() {
			rigidbody = GetComponent<Rigidbody>();
			mainCamera = Camera.main;
		}

		private void FixedUpdate() {
			float accelerationDelta = acceleration * speedMultiplier * Time.deltaTime;

			if (canMove && moveInput != Vector2.zero && accelerationDelta != 0) {
				Vector2 accelerationInput = MakeCameraRelative(moveInput) * accelerationDelta;
				float accelerationMagnitude = accelerationInput.magnitude;

				if (CalculateMovementDirection)
					movementDirection = accelerationInput * (1f / accelerationMagnitude);

				velocity = Mathf.Min(maxVelocity * moveInput.magnitude, velocity + accelerationMagnitude);
			}
			else {
				velocity = Mathf.Max(0, velocity - deceleration * Time.deltaTime);
			}

			ApplyMovement();
		}

		private void ApplyMovement() {
			Vector2 movement2 = movementDirection * (velocity * speedMultiplier * Time.deltaTime);
			movement = new Vector3(movement2.x, 0, movement2.y);

			rigidbody.MovePosition(transform.position + movement);

			if (movement != Vector3.zero && canRotate)
				transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
		}

		public Vector2 MakeCameraRelative(Vector2 movement) {
			// This is needed because the main camera changes between scenes.
			if (mainCamera == null) {
				mainCamera = Camera.main;

				if (mainCamera == null) {
					Debug.LogError("No main camera found", this);
					return movement;
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

			Vector3 cameraRelativeMovement = forward * movement.y + cameraTransform.right * movement.x;
			return new Vector2(cameraRelativeMovement.x, cameraRelativeMovement.z);
		}

		public void MoveInDirection(Vector2 direction) {
			movementDirection = direction;
		}

		public void Move(Vector2 input) {
			moveInput = input;
		}

		public void SetMaxVelocity(float velocity) {
			maxVelocity = velocity;
		}

		public void SetDefaultVelocity() {
			maxVelocity = 6f;
		}
	}
}
