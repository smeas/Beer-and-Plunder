using Rounds;
using UnityEngine;
using UnityEngine.InputSystem;
using World;

namespace Player {
	public class PlayerComponent : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform modelRoot;
		[SerializeField] private PlayerData playerData;

		public int PlayerId { get; private set; }
		public Transform SpawnPoint { get; set; }
		public Transform ModelRoot => modelRoot;
		public MeshRenderer BodyMeshRenderer { get; set; }
		public PlayerData PlayerData { get => playerData; set => playerData = value; }
		public Color PlayerColor { get; set; } = new Color(0.8828125f, 0.8828125f, 0.8828125f);

		private void Start() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += Respawn;
		}

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= Respawn;
		}

		public void Initialize() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			BodyMeshRenderer = GetComponentInChildren<MeshRenderer>();

			if (playerData == null)
				Debug.LogWarning("PlayerData is not set in PlayerComponent");
		}

		public void Respawn() {
			if (SpawnPoint != null)
				transform.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
			else
				transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}
}
