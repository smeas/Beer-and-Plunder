using UnityEngine;
using UnityEngine.InputSystem;
using World;

namespace Player {
	public class PlayerComponent : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform modelRoot;

		public int PlayerId { get; private set; }
		public Transform ModelRoot => modelRoot;

		private void Awake() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;
		}

		public void Respawn() {
			transform.position = Vector3.zero;
		}
	}
}