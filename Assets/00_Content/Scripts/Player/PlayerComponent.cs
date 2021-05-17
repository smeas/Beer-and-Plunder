using System.Collections;
using Rounds;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using World;

namespace Player {
	public class PlayerComponent : MonoBehaviour, IRespawnable {
		[SerializeField] private Transform modelRoot;
		[SerializeField] private PlayerData playerData;
		[SerializeField] private MeshRenderer glowRingRenderer;

		private Color playerColor = new Color(0.8828125f, 0.8828125f, 0.8828125f);
		private NavMeshAgent navMeshAgent;

		public int PlayerId { get; private set; }
		public Transform SpawnPoint { get; set; }
		public Transform ModelRoot => modelRoot;
		public Renderer BodyMeshRenderer { get; set; }
		public Animator CharacterAnimator { get; private set; }
		public PlayerData PlayerData { get => playerData; set => playerData = value; }

		public Color PlayerColor {
			get => playerColor;
			set {
				playerColor = value;

				// Update glow ring color
				Material material = glowRingRenderer.material;
				material.color = new Color(PlayerColor.r, PlayerColor.g, PlayerColor.b, material.color.a);
			}
		}

		private void Awake() {
			if (CharacterAnimator == null)
				CharacterAnimator = GetComponentInChildren<Animator>();
		}

		private void Start() {
			SceneManager.sceneLoaded += HandleOnSceneLoaded;
			HandleOnSceneLoaded(default, default);

			navMeshAgent = GetComponent<NavMeshAgent>();

			PlayerColor = playerColor;
		}

		private void OnDestroy() {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver -= OnRoundOver;

			SceneManager.sceneLoaded -= HandleOnSceneLoaded;
		}

		private void HandleOnSceneLoaded(Scene _, LoadSceneMode __) {
			if (RoundController.Instance != null)
				RoundController.Instance.OnRoundOver += OnRoundOver;
		}

		public void Initialize() {
			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			BodyMeshRenderer = GetComponentInChildren<Renderer>();

			if (playerData == null)
				Debug.LogWarning("PlayerData is not set in PlayerComponent");
		}

		public void SwitchCharacterModel(GameObject prefab) {
			Debug.Assert(modelRoot.childCount == 1, "Model root does not have exactly one child", modelRoot);
			Destroy(modelRoot.GetChild(0).gameObject);

			GameObject model = Instantiate(prefab, modelRoot);
			BodyMeshRenderer = model.GetComponentInChildren<Renderer>();
			CharacterAnimator = model.GetComponentInChildren<Animator>();
		}

		private void OnRoundOver() {
			GetComponentInChildren<PlayerPickUp>().RoundResetHeldItem();

			if (navMeshAgent == null)
				Debug.LogError("Player does not have a navmesh agent");

			navMeshAgent.enabled = true;
			navMeshAgent.SetDestination(SpawnPoint.position);
			StartCoroutine(CoWaitReachSpawnPoint());
		}

		private IEnumerator CoWaitReachSpawnPoint() {
			while (navMeshAgent.pathPending || navMeshAgent.desiredVelocity != Vector3.zero)
				yield return null;

			navMeshAgent.enabled = false;
			transform.rotation = SpawnPoint.rotation;
		}

		public void Respawn() {
			if (SpawnPoint != null)
				transform.SetPositionAndRotation(SpawnPoint.position, SpawnPoint.rotation);
			else
				transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}
}
