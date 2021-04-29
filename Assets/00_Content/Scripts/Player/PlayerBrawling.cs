using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private List<MeshRenderer> bodyParts;
		private PlayerData playerData;

		private void Start() {

			bodyParts = GetComponentsInChildren<MeshRenderer>().ToList();
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
			bodyParts.ForEach(x => x.material = redMaterial);


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

			bodyParts.ForEach(x => x.material = defaultMaterial);
		}

		private IEnumerator MakeInvulnerable() {

			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isInvulnerable = true;

			yield return new WaitForSeconds(playerData.invulnerableDuration);

			isInvulnerable = false;
			StopCoroutine(blinkRoutine);
			bodyParts.ForEach(x => x.enabled = true);
		}

		private IEnumerator BlinkBody() {

			while (true) {
				bodyParts.ForEach(x => x.enabled = !x.enabled);
				yield return new WaitForSeconds(0.2f);
			}
		}

		private IEnumerator StunPlayer() {

			PlayerMovement playerMovement = GetComponent<PlayerMovement>();
			playerMovement.CanMove = false;
			bodyParts.ForEach(x => x.material = yellowMaterial);
			Coroutine blinkRoutine = StartCoroutine(BlinkBody());
			isStunned = true;

			yield return new WaitForSeconds(playerData.stunDuration);

			isStunned = false;
			playerMovement.CanMove = true;
			isInvulnerable = false;
			brawlHealth = 3;
			bodyParts.ForEach(x => x.material = defaultMaterial);
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
