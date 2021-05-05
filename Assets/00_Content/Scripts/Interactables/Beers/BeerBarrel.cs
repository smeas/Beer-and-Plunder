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

			if (carriers[0].Movement.magnitude > 0f && carriers[1].Movement.magnitude > 0f) {

				Vector3 newPos = (carriers[0].transform.position + carriers[1].transform.position) / 2;
				transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);

				LimitMovement(carriers[0]);
				LimitMovement(carriers[1]);
			}
			else if (carriers[0].Movement.magnitude > 0f) {
					Vector3 newPos = (carriers[0].transform.position + carriers[1].transform.position) / 2;
					transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
					LimitMovement(carriers[0]);
			}
			else if (carriers[1].Movement.magnitude > 0f) {
					Vector3 newPos = (carriers[1].transform.position + carriers[0].transform.position) / 2;
					transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
					LimitMovement(carriers[1]);
			}

			foreach (PlayerMovement playerMove in carriers.Values) {
				playerMove.transform.LookAt(new Vector3(transform.position.x, 0, transform.position.z));
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
					playerMove.CanRotate = false;
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
