using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player {
	[CreateAssetMenu(fileName = "new Player", menuName = "Game/PlayerData")]
	public class PlayerData : ScriptableObject {
		[Tooltip("How many hits can be taken when brawling")]
		public float brawlHealth = 3;

		[Tooltip("How often u regenerate 1 health in seconds")]
		public float RegenerationDelay = 3f;

		[Tooltip("How long your stunned when brawlHealth reaches zero")]
		public float stunDuration = 3f;

		[Tooltip("How long you´re invulnerable after being hit")]
		public float invulnerableDuration = 2f;
	}
}
