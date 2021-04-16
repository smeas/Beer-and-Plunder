﻿using UnityEngine;

namespace Interactables {

	[CreateAssetMenu(fileName = "Beer", menuName = "Game/Beer")]
	public class BeerData : ScriptableObject {
		public BeerType type;
		public int cost;
	}
}