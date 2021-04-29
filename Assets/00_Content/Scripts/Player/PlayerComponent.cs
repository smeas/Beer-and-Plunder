using UnityEngine;
using UnityEngine.InputSystem;
using World;

namespace Player {
	public class PlayerComponent : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform modelRoot;

		public int PlayerId { get; private set; }
		public Transform SpawnPoint { get; set; }
		public Transform ModelRoot => modelRoot;

		public void Initialize() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;
		}

		public void Respawn() {
			if (SpawnPoint != null)
				transform.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
			else
				transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}
}