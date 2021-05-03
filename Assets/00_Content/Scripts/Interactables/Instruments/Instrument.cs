using System.Collections.Generic;
using Player;
using UnityEngine;
using Vikings;

namespace Interactables.Instruments {
	[RequireComponent(typeof(AudioSource), typeof(SphereCollider))]
	public class Instrument : PickUp, IUseable, IDesirable {
		[Space]
		[SerializeField] private InstrumentData instrumentData;
		[SerializeField] private GameObject areaField;

		private AudioSource musicSource;
		private SphereCollider sphereCollider;

		private bool isPlaying;
		private GameObject usingPlayer;
		private readonly List<Viking> vikingsInRange = new List<Viking>();

		public DesireType DesireType => instrumentData.desireType;

		private void Awake() {
			musicSource = GetComponent<AudioSource>();
			musicSource.loop = true;

			sphereCollider = GetComponent<SphereCollider>();
			sphereCollider.radius = instrumentData.effectRadius;

			float diameter = instrumentData.effectRadius * 2f;
			areaField.SetActive(false);
			areaField.transform.localScale = new Vector3(diameter, diameter, diameter);
		}

	#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			if (instrumentData == null) return;
			Vector3 position = usingPlayer != null ? usingPlayer.transform.position : transform.position;

			// Visualize effect radius when selected
			Gizmos.DrawWireSphere(position, instrumentData.effectRadius);
		}
	#endif

		public void Use(GameObject user) {
			if (isPlaying) return;

			if (!user.CompareTag("Player")) {
				Debug.LogError($"{nameof(Instrument)} is being used by a non player: {user}", this);
				return;
			}

			usingPlayer = user;
			if (!instrumentData.playerCanMoveWhileUsing)
				usingPlayer.GetComponent<PlayerMovement>().CanMove = false;

			musicSource.clip = instrumentData.music;
			musicSource.Play();

			areaField.SetActive(true);

			isPlaying = true;

			foreach (Viking viking in vikingsInRange) {
				viking.Affect(usingPlayer, this);
			}
		}

		public void EndUse() {
			if (!isPlaying) return;
			isPlaying = false;

			usingPlayer.GetComponent<PlayerMovement>().CanMove = true;

			musicSource.Stop();

			areaField.SetActive(false);

			foreach (Viking viking in vikingsInRange) {
				viking.CancelAffect(usingPlayer, this);
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (other.attachedRigidbody == null) return;

			Viking viking = other.attachedRigidbody.GetComponent<Viking>();

			if (viking != null) {
				vikingsInRange.Add(viking);
				if (isPlaying)
					viking.Affect(usingPlayer, this);
			}
		}

		private void OnTriggerExit(Collider other) {
			if (other.attachedRigidbody == null) return;

			Viking viking = other.attachedRigidbody.GetComponent<Viking>();

			if (viking != null) {
				vikingsInRange.Remove(viking);
				if (isPlaying)
					viking.CancelAffect(usingPlayer, this);
			}
		}
	}
}
