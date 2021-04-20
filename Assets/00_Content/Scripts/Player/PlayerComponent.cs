using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerComponent : MonoBehaviour {
		public int PlayerId { get; private set; }

		private void Awake() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			DontDestroyOnLoad(gameObject);
		}
	}
}