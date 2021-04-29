using System.Collections;
using System.Linq;
using Extensions;
using UnityEngine;
using Vikings;

namespace Player {
	public class PlayerBrawling : MonoBehaviour {

		[SerializeField] private Material defaultMaterial;
		[SerializeField] private Material redMaterial;
		[SerializeField] private Material yellowMaterial;
		[SerializeField] private LayerMask vikingLayer;

		public bool IsStunned { get => isStunned; set => isStunned = value; }

		private bool isStunned;
		private float brawlHealth;
		private bool isRegeneratingHealth;
		private bool isInvulnerable;
		private PlayerData playerData;
		private PlayerComponent playerComponent;

		private void Start() {
			playerComponent = GetComponent<PlayerComponent>();
			playerData = playerComponent.PlayerData;
			brawlHealth = playerData.brawlHealth;

			defaultMaterial = new Material(defaultMaterial) {
				color = playerComponent.PlayerColor
			};
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
			int materialCount = playerComponent.BodyMeshRenderer.sharedMaterials.Length;
			playerComponent.BodyMeshRenderer.sharedMaterials = Enumerable.Repeat(redMaterial, materialCount).ToArray();


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

			int materialCount = playerComponent.BodyMeshRenderer.sharedMaterials.Length;
			playerComponent.BodyMeshRenderer.sharedMaterials = Enumerable.Repeat(defaultMaterial, materialCount).ToArray();
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

			int materialCount = playerComponent.BodyMeshRenderer.sharedMaterials.Length;
			playerComponent.BodyMeshRenderer.sharedMaterials = Enumerable.Repeat(yellowMaterial, materialCount).ToArray();

			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isStunned = true;
			isInvulnerable = true;

			yield return new WaitForSeconds(playerData.stunDuration);

			isStunned = false;
			playerMovement.CanMove = true;
			isInvulnerable = false;
			brawlHealth = 3;
			isRegeneratingHealth = false;

			playerComponent.BodyMeshRenderer.sharedMaterials = Enumerable.Repeat(defaultMaterial, materialCount).ToArray();

			StopCoroutine(blinkRoutine);
			playerComponent.BodyMeshRenderer.enabled = true;
		}

		private void OnCollisionEnter(Collision collision) {

			if (vikingLayer.ContainsLayer(collision.gameObject.layer)) {

				Viking viking = collision.gameObject.GetComponent<Viking>();
				if (viking.IsAttacking)
					TakeBrawlDamage(viking.Data.spinAttackDamage);
			}
		}
	}
}
