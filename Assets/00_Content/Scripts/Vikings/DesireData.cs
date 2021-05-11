using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Desire", menuName = "Game/Desire", order = 0)]
	public class DesireData : ScriptableObject {
		public DesireType type;
		public int randomWeight;
		public GameObject visualisationPrefab;

		[Tooltip("Does the player need to give an item to fulfill this desire")]
		public bool isMaterialDesire = true;

		[HideInInspector]
		public bool isOrder = false;
		[HideInInspector]
		public GameObject visualisationAfterPrefab;

		[Min(0), Tooltip("How long does the desire take to be fulfilled, 0 is instant")]
		public float desireFulfillTime;
	}
}
