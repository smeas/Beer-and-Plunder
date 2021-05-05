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
		[SerializeField] private float carryDistance = 1f;
		
		private Dictionary<int, PlayerMovement> carriers;

		private Collider coll;
		private Rigidbody rb;

		protected override void Start() {
			base.Start();
			OnPickedUp += BeerBarrel_OnPickedUp;
			OnDropped += BeerBarrel_OnDropped;
			coll = GetComponentInChildren<Collider>();
			rb = GetComponent<Rigidbody>();

			carriers = new Dictionary<int, PlayerMovement>();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			OnPickedUp -= BeerBarrel_OnPickedUp;
			OnDropped -= BeerBarrel_OnDropped;
		}

		private void FixedUpdate() {
			if (IsMultiCarried) {
				if (carriers[0].Movement.magnitude > 0f && carriers[1].Movement.magnitude > 0f) {
					Vector3 combinedMovement = carriers[0].Movement + carriers[1].Movement;
					combinedMovement = combinedMovement / 2;
					rb.MovePosition(transform.position + combinedMovement);

					LimitMovement(carriers[0]);
					LimitMovement(carriers[1]);
				}
				else if (carriers[0].Movement.magnitude > 0) {
					LimitMovement(carriers[0]);
				}
				else if (carriers[1].Movement.magnitude > 0) {
					LimitMovement(carriers[1]);
				}

				Debug.Log("p1: " + carriers[0].Movement.magnitude);
				Debug.Log("p2: " + carriers[1].Movement.magnitude);
			}
			else {
				foreach (PlayerMovement playerMove in carriers.Values) {
					playerMove.CanMove = true;
				}
			}
		}

		private void LimitMovement(PlayerMovement playerMovement) {
			float actualDistance = Vector3.Distance(transform.position, playerMovement.transform.position);

			if (actualDistance > carryDistance) {
				Vector3 centerToPosition = playerMovement.transform.position - transform.position;
				centerToPosition.Normalize();
				Vector3 newPosition = transform.position + centerToPosition * carryDistance;
				playerMovement.transform.position = new Vector3(newPosition.x, 0, newPosition.z);
			}
		}

		private void BeerBarrel_OnPickedUp(PickUp pickUp, PlayerComponent playerComponent) {
			PlayerMovement playerMovement = playerComponent.GetComponent<PlayerMovement>();

			if (carriers.Count > 0) {

				coll.enabled = true;

				pickUp.SetParent(null);
				carriers.Add(playerComponent.PlayerId, playerMovement);

				foreach (PlayerMovement playerMove in carriers.Values) {
					playerMove.SetDefaultVelocity();
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
