using Player;
using UnityEngine;

namespace Rounds {
	public class PlayerReadyCard : MonoBehaviour {
		[SerializeField] private GameObject readyText;
		[SerializeField] private Vector2 offset;

		private bool ready;
		private PlayerComponent player;
		private PlayerAnimationDriver animationDriver;

		private void Start() {
			ready = false;
			readyText.SetActive(false);
		}

		private void OnEnable() {
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(PlayerSpawnController.Instance.SpawnPoints[player.PlayerId].position);
			transform.position = screenPoint + new Vector3(offset.x, offset.y, 0f);
		}

		private void OnDisable() {
			Ready = false;
		}

		public bool Ready {
			get => ready;
			set {
				ready = value;
				readyText.SetActive(value);

				if (animationDriver != null)
					animationDriver.Celebrating = value;
			}
		}

		public void TrackPlayer(PlayerComponent targetPlayer) {
			player = targetPlayer;
			animationDriver = player.GetComponentInChildren<PlayerAnimationDriver>();
		}

		public void ToggleReady() {
			Ready = !Ready;
		}
	}
}
