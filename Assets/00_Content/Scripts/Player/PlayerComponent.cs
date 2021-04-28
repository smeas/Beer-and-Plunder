using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Vikings;

namespace Player {
	public class PlayerComponent : MonoBehaviour {
		[SerializeField] private PlayerData playerData;
		[SerializeField] private LayerMask vikingLayer;
		[SerializeField] private Material defaultMaterial;
		[SerializeField] private Material redMaterial;
		[SerializeField] private Material yellowMaterial;

		public int PlayerId { get; private set; }
		public PlayerData PlayerData { get => playerData; set => playerData = value; }
		public float BrawlHealth { get => brawlHealth; set => brawlHealth = value; }
		public bool IsStunned { get => isStunned; set => isStunned = value; }

		private bool isStunned;
		private float brawlHealth;
		private bool isGeneratingHealth;
		private bool isInvulnerable;

		private List<MeshRenderer> bodyParts;

		private void Awake() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			if (playerData == null) {
				Debug.LogWarning("PlayerData is not set in PlayerComponent");
				return;
			}

			brawlHealth = playerData.brawlHealth;
		}

		private void Start() {
			bodyParts = GetComponentsInChildren<MeshRenderer>().ToList();
		}

		private void FixedUpdate() {
			if(brawlHealth < 3 && !isGeneratingHealth) {
				StartCoroutine(GenerateBrawlHealth());
				isGeneratingHealth = true;
			}
		}

		private IEnumerator GenerateBrawlHealth() {
			yield return new WaitForSeconds(playerData.RegenerationDelay);
			brawlHealth++;
			brawlHealth = Mathf.Min(brawlHealth, playerData.brawlHealth);
			isGeneratingHealth = false;
		}

		public void TakeBrawlDamage(float brawlDamage) {

			if (isInvulnerable) {
				return;
			}
			
			brawlHealth -= brawlDamage;
			bodyParts.ForEach(x => x.material = redMaterial);

			StartCoroutine(MakeInvulnerable());
			StartCoroutine(ResetDamageHighlight());

			if(brawlHealth <= 0) {
				StopAllCoroutines();
				StartCoroutine(StunPlayer());
			}
		}

		private IEnumerator ResetDamageHighlight() {

			yield return new WaitForSeconds(1f);

			bodyParts.ForEach(x => x.material = defaultMaterial);
		}

		private IEnumerator MakeInvulnerable() {

			var blinkRoutine = StartCoroutine(BlinkBody());
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
			var blinkRoutine = StartCoroutine(BlinkBody());
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
				var viking = collision.gameObject.GetComponent<Viking>();

				if(viking.IsAttacking)
					TakeBrawlDamage(viking.Data.attackDamage);
			}
		}
	}
}