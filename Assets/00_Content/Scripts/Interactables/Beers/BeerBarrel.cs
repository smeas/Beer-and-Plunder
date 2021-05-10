using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;

namespace Interactables.Beers {

	public class BeerBarrel : PickUp {

		[SerializeField] private float soloCarrySpeedMultiplier = 0.3f;
		[SerializeField] private float carryDistance = 2f;
		[SerializeField] private LayerMask allButSelf;

		private List<PlayerMovement> carriers = new List<PlayerMovement>();
		private List<GameObject> carryCollisionGameObjects = new List<GameObject>();
		
		protected override void Start() {
			base.Start();

			OnPickedUp += BeerBarrel_OnPickedUp;
			OnDropped += BeerBarrel_OnDropped;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			OnPickedUp -= BeerBarrel_OnPickedUp;
			OnDropped -= BeerBarrel_OnDropped;
		}

		public void DropBarrel() {
			PlayerMovement[] carriersArr = carriers.ToArray();

			for (int i = 0; i < carriersArr.Length; i++) {
				PlayerPickUp playerPickup = carriersArr[i].GetComponentInChildren<PlayerPickUp>();
				playerPickup.DropItem();
			}
		}

		private void FixedUpdate() {
			if (!IsMultiCarried)
				return;

			if (carriers[0].Speed > 0 && carriers[1].Speed > 0)
				MoveTogether();
			else if (carriers[0].Speed > 0)
				Rotate(carriers[0], carriers[1]);
			else if (carriers[1].Speed > 0)
				Rotate(carriers[1], carriers[0]);

			foreach (PlayerMovement playerMove in carriers) {
				playerMove.transform.LookAt(new Vector3(transform.position.x, 0, transform.position.z));
			}

			foreach (GameObject carryCollisionObject in carryCollisionGameObjects) {
				carryCollisionObject.transform.position = transform.position;
			}
		}

		private void MoveTogether() {
			Vector2 combinedMovement =
				Vector2.ClampMagnitude((carriers[0].MoveInput + carriers[1].MoveInput), 1f);

			foreach (PlayerMovement playerMovement in carriers) {
				playerMovement.MoveInDirection(combinedMovement);
			}

			MoveBarrel();
		}

		private void MoveBarrel() {
			Vector3 newPos = (carriers[0].transform.position + carriers[1].transform.position) / 2;
			rigidbody.MovePosition(new Vector3(newPos.x, 0, newPos.z));

			Vector3 lookPosition = carriers[0].transform.position;
			Vector3 lookPoint = new Vector3(lookPosition.x, transform.position.y, lookPosition.z);
			transform.LookAt(lookPoint, Vector3.up);
		}

		private void Rotate(PlayerMovement rotatingPlayer, PlayerMovement stillPlayer) {
			MoveBarrel();
			RotatePlayerAroundPlayer(rotatingPlayer, stillPlayer);
		}

		private void RotatePlayerAroundPlayer(PlayerMovement rotatingPlayer, PlayerMovement stillPlayer) {
			Vector3 movingPosition3 = rotatingPlayer.transform.position;
			Vector3 stillPosition3 = stillPlayer.transform.position;

			Vector2 movingPosition2 = new Vector2(movingPosition3.x, movingPosition3.z);
			Vector2 stillPosition2 = new Vector2(stillPosition3.x, stillPosition3.z);

			Vector2 desiredMovement = rotatingPlayer.MakeCameraRelative(rotatingPlayer.MoveInput.normalized) * (rotatingPlayer.Speed * Time.deltaTime);
			Vector2 desiredNextPosition = movingPosition2 + desiredMovement;

			Vector2 correctedPosition = (desiredNextPosition - stillPosition2).normalized * carryDistance;
			rotatingPlayer.transform.position = stillPosition3 + new Vector3(correctedPosition.x, 0, correctedPosition.y);
		}

		private void BeerBarrel_OnPickedUp(PickUp pickUp, PlayerComponent playerComponent) {
			PlayerMovement playerMovement = playerComponent.GetComponent<PlayerMovement>();
			objectCollider.enabled = true;
			carriers.Add(playerMovement);

			if (carriers.Count > 1) {
				pickUp.SetParent(null);

				//TODO -> Fix pickUp bug
				//playerMovement.transform.position = carriers[0].transform.position + carriers[0].transform.forward * carryDistance;
				//playerMovement.transform.LookAt(carriers[0].transform);

				//MoveBarrel();

				rigidbody.isKinematic = false;
				RigidbodyConstraints constraints = rigidbody.constraints;
				constraints |= RigidbodyConstraints.FreezePositionY;
				constraints |= RigidbodyConstraints.FreezeRotationX;
				constraints |= RigidbodyConstraints.FreezeRotationY;
				constraints |= RigidbodyConstraints.FreezeRotationZ;
				rigidbody.constraints = constraints;

				foreach (PlayerMovement playerMove in carriers) {

					GameObject gameObject = new GameObject();
					gameObject.transform.SetParent(playerMove.transform);
					gameObject.layer =  Mathf.FloorToInt(Mathf.Log(allButSelf.value, 2));

					BoxCollider coll = gameObject.AddComponent<BoxCollider>();
					gameObject.transform.position = transform.position;
					coll.size = transform.localScale;
					carryCollisionGameObjects.Add(gameObject);

					playerMove.SpeedMultiplier = 1f;
					playerMove.CanRotate = false;
					playerMove.ShouldCalculateMovementDirection = false;
				}

				IsMultiCarried = true;
				return;
			}

			playerMovement.SpeedMultiplier = soloCarrySpeedMultiplier;
		}

		private void BeerBarrel_OnDropped(PickUp pickUp, PlayerComponent playerComponent) {
			if (IsMultiCarried) {

				foreach (PlayerMovement playerMove in carriers) {
					playerMove.CanRotate = true;
					playerMove.ShouldCalculateMovementDirection = true;
					carryCollisionGameObjects.ForEach(x => Destroy(x));
					carryCollisionGameObjects.Clear();
				}

				carriers.Remove(playerComponent.GetComponent<PlayerMovement>());
				IsMultiCarried = false;

				rigidbody.isKinematic = true;

				RigidbodyConstraints constraints = rigidbody.constraints;
				constraints &= ~RigidbodyConstraints.FreezePositionY;
				constraints &= ~RigidbodyConstraints.FreezeRotationX;
				constraints &= ~RigidbodyConstraints.FreezeRotationY;
				constraints &= ~RigidbodyConstraints.FreezeRotationZ;
				rigidbody.constraints = constraints;

				PlayerMovement remainingCarrier = carriers.First();
				carriers.Clear();
				remainingCarrier.SpeedMultiplier = soloCarrySpeedMultiplier;
				PickUpItem(remainingCarrier.GetComponentInChildren<PlayerPickUp>().PlayerGrabTransform);
				return;
			}

			carriers.First().SpeedMultiplier = 1f;
			carriers.Remove(playerComponent.GetComponent<PlayerMovement>());
		}
	}
}
