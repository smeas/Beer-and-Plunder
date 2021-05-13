using Rounds;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using World;

namespace Player {
	public class PlayerComponent : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform modelRoot;
		[SerializeField] private PlayerData playerData;
		[SerializeField] private MeshRenderer glowRingRenderer;

		public int PlayerId { get; private set; }
		public Transform SpawnPoint { get; set; }
		public Transform ModelRoot => modelRoot;
		public Renderer BodyMeshRenderer { get; set; }
		public PlayerData PlayerData { get => playerData; set => playerData = value; }
		public Color PlayerColor { get; set; } = new Color(0.8828125f, 0.8828125f, 0.8828125f);

		private void Start() {
			SceneManager.sceneLoaded += HandleOnSceneLoaded;
			HandleOnSceneLoaded(default, default);

			// Update glow ring color
			Material material = glowRingRenderer.material;
			Color oldColor = material.color;
			material.color = new Color(PlayerColor.r, PlayerColor.g, PlayerColor.b, oldColor.a);
		}

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= Respawn;

			SceneManager.sceneLoaded -= HandleOnSceneLoaded;
		}

		private void HandleOnSceneLoaded(Scene _, LoadSceneMode __) {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += Respawn;
		}

		public void Initialize() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			BodyMeshRenderer = GetComponentInChildren<Renderer>();

			if (playerData == null)
				Debug.LogWarning("PlayerData is not set in PlayerComponent");
		}

		public void Respawn() {
			//If the items resets first it allows the objects to be dropped & left behind when a new round starts.
			GetComponentInChildren<PlayerPickUp>().RespawnHeldItem();

			if (SpawnPoint != null)
				transform.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
			else
				transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}
}
