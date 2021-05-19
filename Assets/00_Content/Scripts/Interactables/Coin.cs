using System;
using System.Collections.Generic;
using Audio;
using Extensions;
using Taverns;
using UnityEngine;
using World;
using Random = UnityEngine.Random;

namespace Interactables {
	[RequireComponent(typeof(Rigidbody))]
	public class Coin : MonoBehaviour {
		public static List<Coin> AllCoins { get; } = new List<Coin>();

		[SerializeField] private int value = 1;
		[SerializeField] private float upVelocity;
		[SerializeField, Min(0)]
		private float minSpawnVelocity;
		[SerializeField, Min(0)]
		private float maxSpawnVelocity;

		[SerializeField] private float hitSoundVelocityThreshold = 2f;

		private bool isDisplay;
		private Rigidbody rb;

		/// <summary>
		/// Is the coin only for display - meaning it can't be picked up and has no physics?
		/// </summary>
		public bool IsDisplay {
			get => isDisplay;
			set {
				if (isDisplay == value)
					return;

				rb.isKinematic = value;
				if (value)
					AllCoins.SwapRemove(this);
				else
					AllCoins.Add(this);

				isDisplay = value;
			}
		}

		private void Awake() {
			rb = GetComponent<Rigidbody>();
		}

		private void Start() {
			if (isDisplay)
				AllCoins.SwapRemove(this);
		}

		private void OnEnable() {
			AllCoins.Add(this);
		}

		private void OnDisable() {
			AllCoins.SwapRemove(this);
		}

		private void OnTriggerEnter(Collider other) {
			if (isDisplay) return;
			if (other.isTrigger) return;
			if (other.attachedRigidbody == null) return;

			if (other.attachedRigidbody.CompareTag("Goblin")) {
				Goblin goblin = other.attachedRigidbody.GetComponent<Goblin>();
				if (goblin.PickUpCoin(this))
					AudioManager.PlayEffectSafe(SoundEffect.Player_PickUpCoin);
			}
			else if (other.attachedRigidbody.CompareTag("Player")) {
				AudioManager.PlayEffectSafe(SoundEffect.Player_PickUpCoin);
				Destroy(gameObject);

				if (Tavern.Instance != null)
					Tavern.Instance.EarnsMoney(value);
			}
		}

		private void OnCollisionEnter(Collision other) {
			if (isDisplay) return;

			if (other.relativeVelocity.sqrMagnitude >= hitSoundVelocityThreshold * hitSoundVelocityThreshold)
				AudioManager.PlayEffectSafe(SoundEffect.Physics_CoinHit);
		}

		public void RandomThrow() {
			float x = Mathf.Sin(Random.Range(0, Mathf.PI * 2));
			float z = Mathf.Cos(Random.Range(0, Mathf.PI * 2));

			ThrowInDirection(new Vector3(x, 0, z).normalized);
		}

		public void ThrowInDirection(Vector3 direction) {
			Vector3 force = direction * Random.Range(minSpawnVelocity, maxSpawnVelocity);
			force.y = Math.Max(force.y, upVelocity);

			rb.AddForce(force, ForceMode.Impulse);
			rb.AddRelativeTorque(force, ForceMode.Impulse);
		}
	}
}
