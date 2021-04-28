
using UnityEngine;

namespace Interactables.Weapons {

	[CreateAssetMenu(fileName = "Weapon", menuName = "Game/Weapon")]
	public class WeaponData : ScriptableObject {
		public float moodDamage = 25f;
		public float brawlDamage = 1;
		public float knockBackStrength = 50f;
	}
}