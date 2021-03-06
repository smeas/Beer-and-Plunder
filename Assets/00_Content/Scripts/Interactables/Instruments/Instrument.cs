using System.Collections.Generic;
using Audio;
using Player;
using UnityEngine;
using Vikings;

namespace Interactables.Instruments {
	[RequireComponent(typeof(AudioSource), typeof(SphereCollider))]
	public class Instrument : PickUp, IUseable, IDesirable {
		[Space]
		[SerializeField] private InstrumentData instrumentData;
		[SerializeField] private GameObject areaField;
		[SerializeField] private new ParticleSystem particleSystem;
		[SerializeField] private float floorHitSoundVelocityLimit = 1.5f;

		private AudioSource musicSource;
		private SphereCollider sphereCollider;

		private bool isPlaying;
		private GameObject usingPlayer;
		// Tracks all vikings that are in range. Key = the viking, Value = number of triggers in range.
		private Dictionary<Viking, int> vikingsInRange = new Dictionary<Viking, int>();
		//private SoundHandle musicSoundHandle;

		public DesireType DesireType => instrumentData.desireType;
		public bool IsPlaying => isPlaying;

		protected override void Awake() {
			base.Awake();

			musicSource = GetComponent<AudioSource>();
			musicSource.loop = true;

			sphereCollider = GetComponent<SphereCollider>();
			sphereCollider.radius = instrumentData.effectRadius;

			float diameter = instrumentData.effectRadius * 2f;
			areaField.SetActive(false);
			areaField.transform.localScale = new Vector3(diameter, areaField.transform.localScale.y, diameter);

			ParticleSystem.ShapeModule particleSystemShape = particleSystem.shape;
			particleSystemShape.radius = instrumentData.effectRadius - 1f;

			particleSystem.Stop();
		}

		private void FixedUpdate() {
			if (!isPlaying) return;

			foreach (Viking viking in vikingsInRange.Keys)
				viking.Affect(usingPlayer, this);
		}

		private void LateUpdate() {
			// Makes sure the visual effects don't rotate along with the instrument when held in the player's hand.
			// The trigger may still rotate, but as it's a sphere, it shouldn't matter.
			areaField.transform.rotation = Quaternion.identity;
			particleSystem.transform.rotation = Quaternion.Euler(-90, 0, 0);
		}

	#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			if (instrumentData == null) return;
			Vector3 position = usingPlayer != null ? usingPlayer.transform.position : transform.position;

			// Visualize effect radius when selected
			Gizmos.DrawWireSphere(position, instrumentData.effectRadius);
		}
	#endif

		public override void SetParent(Transform newParent) {
			// NOTE: There seem to be some kind of undocumented behaviour where if you reparent the rigidbody,
			// it re-enters all collisions without exiting them first. To mitigate this, clear the list of vikings
			// before reparenting.
			//
			// This means that when reparenting, Enter will be called again for every object that was in the list. This
			// may cause issues if you are doing things in Enter/Exit, so keep this behaviour in mind.
			vikingsInRange.Clear();
			base.SetParent(newParent);
		}

		public void Use(GameObject user) {
			if (isPlaying) return;

			if (!user.CompareTag("Player")) {
				Debug.LogError($"{nameof(Instrument)} is being used by a non player: {user}", this);
				return;
			}

			usingPlayer = user;
			if (!instrumentData.playerCanMoveWhileUsing)
				usingPlayer.GetComponent<PlayerMovement>().BlockMovement();

			musicSource.clip = instrumentData.music;
			musicSource.Play();

			areaField.SetActive(true);
			particleSystem.Play();

			isPlaying = true;

			//musicSoundHandle = AudioManager.PlayEffectSafe(SoundEffect.Instrument_HarpPlay, loop: true);
		}

		public void EndUse() {
			if (!isPlaying) return;
			isPlaying = false;

			usingPlayer.GetComponent<PlayerMovement>().UnblockMovement();

			musicSource.Stop();

			areaField.SetActive(false);
			particleSystem.Stop();

			foreach (Viking viking in vikingsInRange.Keys) {
				viking.CancelAffect(usingPlayer, this);
			}

			//musicSoundHandle?.FadeOutAndStop(0.3f);
		}

		private void OnCollisionEnter(Collision collision) {
			if (collision.gameObject.CompareTag("Ground") && collision.relativeVelocity.y > floorHitSoundVelocityLimit) {
				AudioManager.PlayEffectSafe(SoundEffect.Physics_HarpDrop);
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (other.attachedRigidbody == null) return;

			Viking viking = other.attachedRigidbody.GetComponent<Viking>();

			if (viking == null) return;

			if (vikingsInRange.ContainsKey(viking)) {
				vikingsInRange[viking]++;
			}
			else {
				vikingsInRange[viking] = 1;
			}
		}

		private void OnTriggerExit(Collider other) {
			if (other.attachedRigidbody == null) return;

			Viking viking = other.attachedRigidbody.GetComponent<Viking>();

			if (viking == null) return;

			if (vikingsInRange.ContainsKey(viking)) {
				int value = --vikingsInRange[viking];
				if (value <= 0) {
					vikingsInRange.Remove(viking);

					if (isPlaying)
						viking.CancelAffect(usingPlayer, this);
				}
			}
			else {
				Debug.LogWarning("Unbalanced enter/exits", this);
			}
		}
	}
}
