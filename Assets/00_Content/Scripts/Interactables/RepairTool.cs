using System;
using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables {
	public class RepairTool : PickUp {
		[Space]
		[SerializeField] private GameObject repairProgressCanvas;
		[SerializeField] private Image repairProgressImage;
		[SerializeField] private float floorHitSoundVelocityLimit = 1.5f;
		[SerializeField] private float floorHitDelay = 2f;

		private bool isRepairing;
		private float repairDuration;
		private float repairTimer;
		private float floorHitTimer;
		private bool hasHitFloor;

		private SoundHandle soundHandle;

		public bool IsRepairing => isRepairing;

		public event Action RepairDone;

		protected override void Start() {
			base.Start();
			repairProgressCanvas.transform.SetParent(null);
		}

		private void Update() {
			if (hasHitFloor)
				floorHitTimer += Time.deltaTime;

			if (!isRepairing) return;

			repairTimer += Time.deltaTime;
			floorHitTimer += Time.deltaTime;
			if (repairTimer >= repairDuration) {
				isRepairing = false;
				repairProgressCanvas.SetActive(false);
				soundHandle?.FadeOutAndStop(0.2f);

				RepairDone?.Invoke();
			}
			else {
				repairProgressImage.fillAmount = repairTimer / repairDuration;
			}
		}

		public void BeginRepairing(float duration, Vector3 progressBarPosition) {
			Debug.Assert(!isRepairing, "Already repairing!", this);
			isRepairing = true;
			repairDuration = duration;
			repairTimer = 0f;
			repairProgressImage.fillAmount = 0f;
			repairProgressCanvas.SetActive(true);
			repairProgressCanvas.transform.SetPositionAndRotation(progressBarPosition, Quaternion.identity);
		}

		public void EndRepairing() {
			if (!isRepairing) return;

			isRepairing = false;
			repairProgressCanvas.SetActive(false);
			soundHandle?.FadeOutAndStop(0.2f);
		}

		private void OnCollisionEnter(Collision collision) {
			if (collision.gameObject.CompareTag("Ground") && collision.relativeVelocity.y > floorHitSoundVelocityLimit) {
				if (!hasHitFloor) {
					AudioManager.PlayEffectSafe(SoundEffect.Physics_HammerDrop);
					hasHitFloor = true;
				}

				if (floorHitTimer >= floorHitDelay) {
					floorHitTimer = 0;
					hasHitFloor = false;
				}
			}
		}
	}
}