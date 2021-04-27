using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Desire", menuName = "Game/Desire", order = 0)]
	public class DesireData : ScriptableObject {
		public DesireType type;
		public GameObject visualisationPrefab;

		// Make different desires weighted?
	}
}
