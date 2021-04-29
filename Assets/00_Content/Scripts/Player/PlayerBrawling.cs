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

		public float BrawlHealth { get => brawlHealth; set => brawlHealth = value; }
		public bool IsStunned { get => isStunned; set => isStunned = value; }

		private bool isStunned;
		private float brawlHealth;
		private bool isRegeneratingHealth;
		private bool isInvulnerable;
		private MeshRenderer bodyMeshRenderer;
		private PlayerData playerData;

		private void Start() {

			bodyMeshRenderer = GetComponentInChildren<MeshRenderer>();
			playerData = GetComponent<PlayerComponent>().PlayerData;
			brawlHealth = playerData.brawlHealth;
		}

		private void FixedUpdate() {

			if (brawlHealth < 3 && !isRegeneratingHealth) {
				StartCoroutine(GenerateBrawlHealth());
			}
		}

		private IEnumerator GenerateBrawlHealth() {

			isRegeneratingHealth = true;
			yield return new WaitForSeconds(playerData.regenerationDelay);
			brawlHealth++;
			brawlHealth = Mathf.Min(brawlHealth, playerData.brawlHealth);
			isRegeneratingHealth = false;
		}

		public void TakeBrawlDamage(float brawlDamage) {

			if (isInvulnerable) {
				return;
			}

			brawlHealth -= brawlDamage;
			int materialCount = bodyMeshRenderer.sharedMaterials.Length;
			bodyMeshRenderer.sharedMaterials = Enumerable.Repeat(redMaterial, materialCount).ToArray();


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

			int materialCount = bodyMeshRenderer.sharedMaterials.Length;
			bodyMeshRenderer.sharedMaterials = Enumerable.Repeat(defaultMaterial, materialCount).ToArray();
		}

		private IEnumerator MakeInvulnerable() {

			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isInvulnerable = true;

			yield return new WaitForSeconds(playerData.invulnerableDuration);

			isInvulnerable = false;
			StopCoroutine(blinkRoutine);
			bodyMeshRenderer.enabled = true;
		}

		private IEnumerator BlinkBody() {

			while (true) {
				bodyMeshRenderer.enabled = !bodyMeshRenderer.enabled;
				yield return new WaitForSeconds(0.2f);
			}
		}

		private IEnumerator StunPlayer() {

			PlayerMovement playerMovement = GetComponent<PlayerMovement>();
			playerMovement.CanMove = false;

			int materialCount = bodyMeshRenderer.sharedMaterials.Length;
			bodyMeshRenderer.sharedMaterials = Enumerable.Repeat(yellowMaterial, materialCount).ToArray();

			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isStunned = true;

			yield return new WaitForSeconds(playerData.stunDuration);

			isStunned = false;
			playerMovement.CanMove = true;
			isInvulnerable = false;
			brawlHealth = 3;

			bodyMeshRenderer.sharedMaterials = Enumerable.Repeat(defaultMaterial, materialCount).ToArray();

			StopCoroutine(blinkRoutine);
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
