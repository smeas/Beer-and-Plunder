using System;
using System.Collections;
using Extensions;
using Interactables.Weapons;
using UnityEngine;
using Vikings;

namespace Player {
	public class PlayerBrawling : MonoBehaviour {

		[SerializeField] private LayerMask vikingHandsLayer;

		public bool IsStunned { get => isStunned; set => isStunned = value; }

		private bool isStunned;
		private float brawlHealth;
		private bool isRegeneratingHealth;
		private bool isInvulnerable;
		private PlayerData playerData;
		private PlayerComponent playerComponent;
		private Axe heldAxe;

		public event Action OnAttack;

		private void Start() {
			playerComponent = GetComponent<PlayerComponent>();
			playerData = playerComponent.PlayerData;
			brawlHealth = playerData.brawlHealth;

			// Event handlers for keeping track of the held axe and forwarding its OnAttack event
			PlayerPickUp playerPickUp = GetComponentInChildren<PlayerPickUp>();
			playerPickUp.OnItemPickedUp += pickUp => {
				if (pickUp is Axe axe) {
					heldAxe = axe;
					heldAxe.OnAttack += HandleOnHeldAxeAttack;
				}
			};

			playerPickUp.OnItemDropped += pickUp => {
				if (pickUp is Axe axe) {
					Debug.Assert(heldAxe == axe, "Dropped axe was not the held axe");
					heldAxe.OnAttack -= HandleOnHeldAxeAttack;
					heldAxe = null;
				}
			};
		}

		private void HandleOnHeldAxeAttack() {
			OnAttack?.Invoke();
		}

		private void FixedUpdate() {

			if (brawlHealth < 3 && !isRegeneratingHealth && !isStunned && !isInvulnerable) {
				StartCoroutine(RegenerateBrawlHealth());
			}
		}

		private IEnumerator RegenerateBrawlHealth() {

			isRegeneratingHealth = true;
			yield return new WaitForSeconds(playerData.regenerationDelay);

			if (!isStunned) {
				brawlHealth++;
				brawlHealth = Mathf.Min(brawlHealth, playerData.brawlHealth);
			}

			isRegeneratingHealth = false;
		}

		public void TakeBrawlDamage(float brawlDamage) {

			if (isInvulnerable) {
				return;
			}

			brawlHealth -= brawlDamage;

			// TODO: Delete these
			playerComponent.BodyMeshRenderer.material.color = Color.red;

			if (brawlHealth <= 0) {
				StopAllCoroutines();
				StartCoroutine(StunPlayer());
			}
			else {
				StartCoroutine(MakeInvulnerable());
				StartCoroutine(ResetDamageHighlight());
			}
		}

		private IEnumerator ResetDamageHighlight() {

			yield return new WaitForSeconds(1f);

			playerComponent.BodyMeshRenderer.material.color = Color.white;
		}

		private IEnumerator MakeInvulnerable() {

			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isInvulnerable = true;

			yield return new WaitForSeconds(playerData.invulnerableDuration);

			isInvulnerable = false;
			StopCoroutine(blinkRoutine);
			playerComponent.BodyMeshRenderer.enabled = true;
		}

		private IEnumerator BlinkBody() {

			while (true) {
				playerComponent.BodyMeshRenderer.enabled = !playerComponent.BodyMeshRenderer.enabled;
				yield return new WaitForSeconds(0.2f);
			}
		}

		private IEnumerator StunPlayer() {

			PlayerMovement playerMovement = GetComponent<PlayerMovement>();
			playerMovement.CanMove = false;

			playerComponent.BodyMeshRenderer.material.color = Color.yellow;

			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isStunned = true;
			isInvulnerable = true;

			yield return new WaitForSeconds(playerData.stunDuration);

			isStunned = false;
			playerMovement.CanMove = true;
			isInvulnerable = false;
			brawlHealth = 3;
			isRegeneratingHealth = false;

			playerComponent.BodyMeshRenderer.material.color = Color.white;

			StopCoroutine(blinkRoutine);
			playerComponent.BodyMeshRenderer.enabled = true;
		}

		private void OnTriggerEnter(Collider other) {
			if (vikingHandsLayer.ContainsLayer(other.gameObject.layer)) {
				Viking viking = other.gameObject.GetComponentInParent<Viking>();
				if (viking.IsAttacking)
					TakeBrawlDamage(viking.Data.damage);
			}
		}
	}
}
