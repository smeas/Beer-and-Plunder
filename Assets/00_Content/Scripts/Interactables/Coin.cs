using System.Collections.Generic;
using Audio;
using Taverns;
using UnityEngine;
using World;

namespace Interactables {
	[RequireComponent(typeof(Rigidbody))]
	public class Coin : MonoBehaviour {
		public static HashSet<Coin> AllCoins { get; } = new HashSet<Coin>();

		[SerializeField] private int value = 1;
		[SerializeField] private float duration;

		[SerializeField] private float upVelocity;
		[SerializeField, Min(0)]
		private float minSpawnVelocity;
		[SerializeField, Min(0)]
		private float maxSpawnVelocity;

		[SerializeField] private float hitSoundVelocityThreshold = 2f;

		private float durationTimer;

		private void Start() {
			Rigidbody rb = GetComponent<Rigidbody>();

			float x = Mathf.Sin(Random.Range(0, Mathf.PI * 2));
			float z = Mathf.Cos(Random.Range(0, Mathf.PI * 2));

			Vector3 direction = new Vector3(x, 0, z).normalized * Random.Range(minSpawnVelocity, maxSpawnVelocity);
			direction.y = upVelocity;

			rb.AddForce(direction, ForceMode.Impulse);
			rb.AddRelativeTorque(direction, ForceMode.Impulse);
		}

		private void OnEnable() {
			AllCoins.Add(this);
		}

		private void OnDisable() {
			AllCoins.Remove(this);
		}

		private void Update() {
			durationTimer += Time.deltaTime;

			if (durationTimer >= duration) {
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (other.isTrigger) return;
			if (other.attachedRigidbody == null) return;

			bool pickedUp = false;

			if (other.attachedRigidbody.CompareTag("Goblin")) {
				Goblin goblin = other.attachedRigidbody.GetComponent<Goblin>();
				if (goblin.CanPickUpCoins) {
					pickedUp = true;
					goblin.Coins++;
				}
			}
			else if (other.attachedRigidbody.CompareTag("Player")) {
				pickedUp = true;

				if (Tavern.Instance != null)
					Tavern.Instance.EarnsMoney(value);
			}

			if (pickedUp) {
				AudioManager.PlayEffectSafe(SoundEffect.Player_PickUpCoin);
				Destroy(gameObject);
			}
		}

		private void OnCollisionEnter(Collision other) {
			if (other.relativeVelocity.sqrMagnitude >= hitSoundVelocityThreshold * hitSoundVelocityThreshold)
				AudioManager.PlayEffectSafe(SoundEffect.Physics_CoinHit);
		}
	}
}
