using Audio;
using Interactables;
using UnityEngine;

namespace Player {
	public class PlayerAnimationEvents : MonoBehaviour {
		private PlayerPickUp playerPickUp;

		private void Start() {
			PlayerComponent player = GetComponentInParent<PlayerComponent>();
			playerPickUp = player.GetComponentInChildren<PlayerPickUp>();
		}

		public void HandleOnHammerRepairHit() {
			if (playerPickUp.PickedUpItem is RepairTool)
				AudioManager.PlayEffectSafe(SoundEffect.Player_HammerRepairHit);
		}
	}
}
