using System.Collections.Generic;
using Player;
using UnityEngine;
using Vikings;

namespace Interactables.Instruments {
	[RequireComponent(typeof(AudioSource))]
	public class Instrument : PickUp, IUseable {
		[SerializeField] private InstrumentData instrumentData;

		private AudioSource musicSource;

		private bool isPlaying;
		private GameObject usingPlayer;
		private readonly List<Viking> vikingsInRange = new List<Viking>();

		private void Awake() {
			musicSource = GetComponent<AudioSource>();
			musicSource.loop = true;
		}

		private void FixedUpdate() {
			if (!isPlaying) return;

			UpdateVikingsInRange();
			print($"Music affecting {vikingsInRange.Count} vikings...");
			foreach (Viking viking in vikingsInRange) {
				// TODO: Affect viking
			}
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

			// TODO: Display some kind of visual that shows the area of effect.

			isPlaying = true;
		}

		public void EndUse() {
			if (!isPlaying) return;
			isPlaying = false;

			usingPlayer.GetComponent<PlayerMovement>().CanMove = true;

			musicSource.Stop();
		}

		private void UpdateVikingsInRange() {
			vikingsInRange.Clear();
			Collider[] colliders = Physics.OverlapSphere(usingPlayer.transform.position, instrumentData.effectRadius);

			foreach (Collider col in colliders) {
				if (col.attachedRigidbody == null)
					continue;

				Viking viking = col.attachedRigidbody.GetComponent<Viking>();
				if (viking != null)
					vikingsInRange.Add(viking);
			}
		}
	}
}
