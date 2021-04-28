using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerComponent : MonoBehaviour {
		[SerializeField] private PlayerData playerData;
		public int PlayerId { get; private set; }
		public PlayerData PlayerData { get => playerData; set => playerData = value; }
		public float BrawlHealth { get => brawlHealth; set => brawlHealth = value; }

		private float brawlHealth;

		private bool isGeneratingHealth;
		private bool isInvulnerable;

		private void Awake() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			if (playerData == null) {
				Debug.LogWarning("PlayerData is not set in PlayerComponent");
				return;
			}

			brawlHealth = playerData.brawlHealth;
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

			if (isInvulnerable)
				return;
			
			brawlHealth -= brawlDamage;
			Debug.Log("Player taking damage");

			StartCoroutine(MakeInvulnerable());

			if(brawlHealth <= 0) {
				StartCoroutine(StunPlayer());
			}
		}

		private IEnumerator MakeInvulnerable() {
			isInvulnerable = true;
			yield return new WaitForSeconds(playerData.invulnerableDuration);
			isInvulnerable = false;
		}

		private IEnumerator StunPlayer() {
			PlayerMovement playerMovement = GetComponent<PlayerMovement>();
			playerMovement.CanMove = false;

			yield return new WaitForSeconds(playerData.stunDuration);

			playerMovement.CanMove = true;
			brawlHealth = 3;
		}
	}
}