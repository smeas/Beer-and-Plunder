using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using Player;
using System.Linq;

namespace Interactables.Beers {

	public class BeerBarrel : PickUp {

		[SerializeField] private float soloCarryVelocity = 2f;
		[SerializeField] private float carryForwardOffset = 0.2f;
		[SerializeField] private float carryDistance = 2f;
		
		private Dictionary<int, PlayerMovement> carriers;
		private Collider coll;

		protected override void Start() {
			base.Start();

			OnPickedUp += BeerBarrel_OnPickedUp;
			OnDropped += BeerBarrel_OnDropped;
			coll = GetComponentInChildren<Collider>();
			carriers = new Dictionary<int, PlayerMovement>();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			OnPickedUp -= BeerBarrel_OnPickedUp;
			OnDropped -= BeerBarrel_OnDropped;
		}

		private void FixedUpdate() {
			if (!IsMultiCarried) 
				return;

			if (carriers[0].Velocity > 0 && carriers[1].Velocity > 0) {

				Vector2 combinedMovement =  Vector2.ClampMagnitude((carriers[0].Movement + carriers[1].Movement),1f);
				foreach (PlayerMovement playerMovement in carriers.Values) {
					playerMovement.MoveInDirection(combinedMovement);
				}

				Vector3 newPos = (carriers[0].transform.position + carriers[1].transform.position) / 2;
				transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
			}
			else if (carriers[0].Velocity > 0) {
					Vector3 newPos = (carriers[0].transform.position + carriers[1].transform.position) / 2;
					transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
					Debug.Log(RotateAroundPlayer(carriers[0], carriers[1]));

			}
			else if (carriers[1].Velocity > 0) {
					Vector3 newPos = (carriers[1].transform.position + carriers[0].transform.position) / 2;
					transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
					RotateAroundPlayer(carriers[1], carriers[0]);
			}
			else {
				foreach (PlayerMovement playerMovement in carriers.Values) {
					playerMovement.MoveInDirection(Vector2.zero);
				}
			}


			foreach (PlayerMovement playerMove in carriers.Values) {
				playerMove.transform.LookAt(new Vector3(transform.position.x, 0, transform.position.z));
			}

		}

		private void LimitMovement(PlayerMovement movingPlayer) {
			Vector3 direction = movingPlayer.transform.position - transform.position;
			direction = Vector3.ClampMagnitude(direction, carryDistance);
			movingPlayer.transform.position = transform.position + direction;
		}

		private Vector2 RotateAroundPlayer(PlayerMovement movingPlayer, PlayerMovement stillPlayer) {

			
			var moveDirection = movingPlayer.MakeCameraRelative(movingPlayer.Movement.normalized) * movingPlayer.Velocity;
			var movingPosition = new Vector2(movingPlayer.transform.position.x, movingPlayer.transform.position.z);

			var nextPosition = movingPosition + moveDirection;
			var stillPosition = new Vector2(stillPlayer.transform.position.x, stillPlayer.transform.position.z);

			var clamped = (nextPosition - stillPosition).normalized * carryDistance;
			var direction = Vector2.ClampMagnitude((clamped - movingPosition), 1f);
			//var nextDirection = (nextPosition - stillPlayer.transform.position).normalized;
			//var something = stillPlayer.transform.position + nextDirection * carryDistance;
			//var x = (something - movingPlayer.transform.position).normalized;

			movingPlayer.MoveInDirection(direction);

			return direction;
		}

		private void BeerBarrel_OnPickedUp(PickUp pickUp, PlayerComponent playerComponent) {
			PlayerMovement playerMovement = playerComponent.GetComponent<PlayerMovement>();

			if (carriers.Count > 0) {

				coll.enabled = true;
				pickUp.SetParent(null);
				carriers.Add(playerComponent.PlayerId, playerMovement);

				foreach (PlayerMovement playerMove in carriers.Values) {
					playerMove.SetDefaultVelocity();
					playerMove.CanRotate = false;
					playerMove.CalculateMovementDirection = false;
				}

				IsMultiCarried = true;
				return;
			}

			carriers.Add(playerComponent.PlayerId, playerMovement);
			carriers.First().Value.SetMaxVelocity(soloCarryVelocity);
			pickUp.transform.localPosition += new Vector3(0, 0, carryForwardOffset);
			coll.enabled = true;
		}

		private void BeerBarrel_OnDropped(PickUp pickUp, PlayerComponent playerComponent) {
			PlayerMovement playerMovement = playerComponent.GetComponent<PlayerMovement>();

			if (IsMultiCarried) {

				foreach (PlayerMovement playerMove in carriers.Values) {
					playerMove.CanRotate = true;
					playerMove.CalculateMovementDirection = true;
				}

				carriers.Remove(playerComponent.PlayerId);
				IsMultiCarried = false;
				PlayerMovement remainingCarrier = carriers.First().Value;
				carriers.Clear();
				remainingCarrier.SetMaxVelocity(soloCarryVelocity);
				PickUpItem(remainingCarrier.GetComponentInChildren<PlayerPickUp>().PlayerGrabTransform);
				return;
			}

			carriers.First().Value.SetDefaultVelocity();
			carriers.Remove(playerComponent.PlayerId);
		}
	}
}
