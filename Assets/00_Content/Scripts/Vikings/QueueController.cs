using System.Collections.Generic;
using UnityEngine;

namespace Vikings {
	public class QueueController : MonoBehaviour {
		[SerializeField] private int queueCapacity = 3;
		[SerializeField] private Vector3 queueDirection = new Vector3(1, 0, 0);
		[SerializeField] private float vikingSpacing = 1.5f;
		[SerializeField] private float vikingSpeed = 1.5f;

		private List<Viking> queue = new List<Viking>();

		public int QueueCapacity => queueCapacity;
		public int QueueSize => queue.Count;

		private void Update() {
			UpdateQueue();
		}

		public void AddToQueue(Viking viking) {
			queue.Add(viking);
			viking.LeaveQueue += HandleOnVikingLeaveQueue;

			// Put the viking one step behind the last spot in the queue
			Vector3 desiredPosition = transform.position + vikingSpacing * queueCapacity * queueDirection;
			viking.transform.position = desiredPosition;
			viking.transform.rotation = Quaternion.LookRotation(-queueDirection);
		}

		private void HandleOnVikingLeaveQueue(Viking sender) {
			queue.Remove(sender);
		}

		private void UpdateQueue() {
			Vector3 desiredPosition = transform.position;

			for (int i = 0; i < queue.Count; i++) {
				Viking viking = queue[i];
				viking.QueuePosition = i;

				Vector3 currentPosition = viking.transform.position;
				Vector3 newPosition =
					Vector3.MoveTowards(currentPosition, desiredPosition, vikingSpeed * Time.deltaTime);
				viking.transform.position = newPosition;

				desiredPosition += queueDirection * vikingSpacing;
			}
		}
	}
}