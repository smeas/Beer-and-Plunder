using UnityEngine;

namespace Player {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMovement : MonoBehaviour {
		[SerializeField] private Collider movementCollider;
		[SerializeField] private float acceleration = 48f;
		[SerializeField] private float deceleration = 48f;
		[SerializeField] private float maxVelocity = 6f;
		[SerializeField] private float speedMultiplier = 1f;
		[SerializeField] private bool canMove = true;

		private new Rigidbody rigidbody;
		private Camera mainCamera;

		private Vector2 moveInput;
		private Vector2 movementDirection;
		private float speed;

		public float Speed => speed;
		public Vector2 MoveInput => moveInput;
		public Vector3 Velocity => new Vector3(movementDirection.x * speed * speedMultiplier, 0f, movementDirection.y * speed * speedMultiplier);
		public Collider MovementCollider => movementCollider;
		public bool DoMovement { get; set; } = true;

		public float SpeedMultiplier {
			get => speedMultiplier;
			set => speedMultiplier = value;
		}

		public bool CanMove {
			get => canMove;
			set => canMove = value;
		}

		private void Start() {
			rigidbody = GetComponent<Rigidbody>();
			mainCamera = Camera.main;
		}

		private void FixedUpdate() {
			float accelerationDelta = acceleration * speedMultiplier * Time.deltaTime;

			if (canMove && moveInput != Vector2.zero && accelerationDelta != 0) {
				Vector2 accelerationInput = MakeCameraRelative(moveInput) * accelerationDelta;
				float accelerationMagnitude = accelerationInput.magnitude;

				movementDirection = accelerationInput * (1f / accelerationMagnitude);

				speed = Mathf.Min(maxVelocity * moveInput.magnitude, speed + accelerationMagnitude);
			}
			else {
				speed = Mathf.Max(0, speed - deceleration * Time.deltaTime);
			}

			if (DoMovement)
				ApplyMovement();
		}

		private void ApplyMovement() {
			if (speed == 0 || movementDirection == Vector2.zero) return;

			Vector2 movement2 = movementDirection * (speed * speedMultiplier * Time.deltaTime);
			if (movement2 == Vector2.zero)
				return;

			Vector3 movement3 = new Vector3(movement2.x, 0, movement2.y);

			rigidbody.MovePosition(transform.position + movement3);

			transform.rotation = Quaternion.LookRotation(movement3, Vector3.up);
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

		public void Move(Vector2 input) {
			moveInput = input;
		}
	}
}
