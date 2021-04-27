using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interactables {
	public class RepairTool : PickUp {
		[Space]
		[SerializeField] private GameObject repairProgressCanvas;
		[SerializeField] private Image repairProgressImage;

		private bool isRepairing;
		private float repairDuration;
		private float repairTimer;

		public bool IsRepairing => isRepairing;

		public event Action RepairDone;

		protected override void Start() {
			base.Start();
			repairProgressCanvas.transform.SetParent(null);
		}

		private void Update() {
			if (!isRepairing) return;

			repairTimer += Time.deltaTime;
			if (repairTimer >= repairDuration) {
				isRepairing = false;
				repairProgressCanvas.SetActive(false);

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
			isRepairing = false;
			repairProgressCanvas.SetActive(false);
		}
	}
}