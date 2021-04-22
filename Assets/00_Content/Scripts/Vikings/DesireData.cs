﻿using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Desire", menuName = "Game/Desire", order = 0)]
	public class DesireData : ScriptableObject {
		public DesireType type;

		// Make different desires weighted?
	}
}