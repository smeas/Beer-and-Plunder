using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.Animations;

namespace Interactables.Beers {
	[DefaultExecutionOrder(10)]
	public class BeerBarrel : PickUp {
		[SerializeField] private float soloCarrySpeedMultiplier = 0.3f;
		[SerializeField] private Vector3 soloCarryRotation;
		[SerializeField] private float multiCarrySpeedMultiplier = 0.8f;
		[SerializeField] private Vector3 multiCarryRotation;
		[SerializeField] private BoxCollider multiCarryCollider;
		[SerializeField] private float carryRadius = 1.5f;

		private Transform carryPoint1;
		private Transform carryPoint2;

		private List<PlayerMovement> carriers = new List<PlayerMovement>();

		public override bool IsHeavy => !IsMultiCarried;

		protected override void Start() {
			base.Start();

			OnPickedUp += HandleOnPickedUp;
			OnDropped += HandleOnDrop;

			CreateCarryPoints();
		}

		private void FixedUpdate() {
			if (!IsMultiCarried) return;

			foreach (PlayerMovement carrier in carriers) {
				Vector3 position = transform.position;
				carrier.transform.LookAt(new Vector3(position.x, carrier.transform.position.y, position.z));
			}

			if (carriers[0].MoveInput != Vector2.zero && carriers[1].MoveInput != Vector2.zero)
				Move();
			else if (carriers[0].MoveInput != Vector2.zero)
				Rotate(carriers[0], carriers[1]);
			else if (carriers[1].MoveInput != Vector2.zero)
				Rotate(carriers[1], carriers[0]);
		}

		private void CreateCarryPoints() {
			carryPoint1 = new GameObject("CarryPoint1").transform;
			carryPoint1.parent = transform;
			carryPoint1.localPosition = new Vector3(0f, 0f, -carryRadius);

			carryPoint2 = new GameObject("CarryPoint2").transform;
			carryPoint2.parent = transform;
			carryPoint2.localPosition = new Vector3(0f, 0f, carryRadius);
		}

		private void Move() {
			if (!carriers[0].CanMove || !carriers[1].CanMove) return;

			Vector3 avgVelocity = (carriers[0].Velocity + carriers[1].Velocity) / 2;

			rigidbody.MovePosition(rigidbody.position + avgVelocity * Time.deltaTime);
		}

		private void Rotate(PlayerMovement rotatingPlayer, PlayerMovement stillPlayer) {
			if (!carriers[0].CanMove || !carriers[1].CanMove) return;

			Vector3 stillPosition = stillPlayer.transform.position;

			Vector3 playerDirection3 = rotatingPlayer.transform.position - stillPosition;
			Vector2 playerDirection2 = new Vector2(playerDirection3.x, playerDirection3.z);

			Vector2 tangent = -Vector2.Perpendicular(playerDirection2).normalized;

			Vector2 velocity2 = new Vector2(rotatingPlayer.Velocity.x,rotatingPlayer.Velocity.z);
			float moveDistance = Vector2.Dot(velocity2, tangent) * Time.deltaTime;

			float rotateAngle = moveDistance / carryRadius;

			transform.RotateAround(stillPosition, Vector3.up, rotateAngle * Mathf.Rad2Deg);
		}

		private void HandleOnPickedUp(PickUp _, PlayerComponent player) {
			PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
			carriers.Add(playerMovement);
			Transform myTransform = transform;

			Physics.IgnoreCollision(objectCollider, playerMovement.MovementCollider, true);

			if (carriers.Count > 1) {
				myTransform.localEulerAngles = multiCarryRotation;
				myTransform.parent = null;

				// Set the height due to different models carrying at different heights
				float desiredY = multiCarryCollider.size.y / 2;
				Vector3 myPosition = myTransform.position;
				myTransform.position = new Vector3(myPosition.x, desiredY, myPosition.z);

				rigidbody.isKinematic = false;

				foreach (PlayerMovement carrier in carriers) {
					carrier.SpeedMultiplier = multiCarrySpeedMultiplier;
					carrier.DoMovement = false;

					Physics.IgnoreCollision(multiCarryCollider, carrier.MovementCollider, true);
				}

				transform.rotation = Quaternion.LookRotation(carriers[0].transform.forward, carriers[0].transform.up);
				LockPosition(carriers[0], carryPoint1);
				LockPosition(carriers[1], carryPoint2);

				rigidbody.constraints |= RigidbodyConstraints.FreezePositionY
				                         | RigidbodyConstraints.FreezeRotationX
				                         | RigidbodyConstraints.FreezeRotationZ;

				multiCarryCollider.enabled = true;

				IsMultiCarried = true;
				return;
			}

			myTransform.localEulerAngles = soloCarryRotation;
			objectCollider.enabled = true;
			playerMovement.SpeedMultiplier = soloCarrySpeedMultiplier;
		}

		private static void LockPosition(PlayerMovement player, Transform carryPoint) {
			Vector3 carryPosition = carryPoint.position;
			carryPoint.position = new Vector3(
				carryPosition.x,
				player.transform.position.y,
				carryPosition.z
			);

			PositionConstraint parent = player.gameObject.AddComponent<PositionConstraint>();
			parent.AddSource(new ConstraintSource {sourceTransform = carryPoint, weight = 1});
			parent.constraintActive = true;
		}

		private void HandleOnDrop(PickUp _, PlayerComponent player) {
			PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

			Physics.IgnoreCollision(objectCollider, playerMovement.MovementCollider, false);

			if (IsMultiCarried) {
				foreach (PlayerMovement carrier in carriers) {
					carrier.SpeedMultiplier = 1f;
					carrier.DoMovement = true;

					Physics.IgnoreCollision(multiCarryCollider, carrier.MovementCollider, false);

					Destroy(carrier.GetComponent<PositionConstraint>());
				}

				rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY
				                         & ~RigidbodyConstraints.FreezeRotationX
				                         & ~RigidbodyConstraints.FreezeRotationZ;

				multiCarryCollider.enabled = false;
				objectCollider.enabled = true;

				carriers.Remove(playerMovement);
				PlayerMovement soloCarrier = carriers.First();
				soloCarrier.SpeedMultiplier = soloCarrySpeedMultiplier;

				MoveToPoint(soloCarrier.GetComponentInChildren<PlayerPickUp>().PlayerGrabTransform);

				IsMultiCarried = false;
				transform.localEulerAngles = soloCarryRotation;
				return;
			}

			playerMovement.SpeedMultiplier = 1f;
			carriers.Remove(playerMovement);
		}

		public void Release() {
			for (int i = carriers.Count - 1; i >= 0; i--) {
				PlayerPickUp playerPickup = carriers[i].GetComponentInChildren<PlayerPickUp>();
				playerPickup.DropItem();
			}
		}
	}
}
