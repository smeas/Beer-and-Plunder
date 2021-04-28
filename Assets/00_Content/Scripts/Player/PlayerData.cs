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
	}
}
