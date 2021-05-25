using Audio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player {
	public class PlayerAnimationEvents : MonoBehaviour {
		public void HandleOnHammerRepairHit() {
			AudioManager.PlayEffectSafe(SoundEffect.Player_HammerRepairHit);
		}
	}
}
