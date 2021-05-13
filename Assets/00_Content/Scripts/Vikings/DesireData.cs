using UnityEngine;

namespace Vikings {
	[CreateAssetMenu(fileName = "new Desire", menuName = "Game/Desire", order = 0)]
	public class DesireData : ScriptableObject {
		public DesireType type;
		public int randomWeight;
		public Sprite visualisationSprite;

		[Tooltip("Does the player need to give an item to fulfill this desire")]
		public bool isMaterialDesire = true;
		[Tooltip("Should the viking throw the item when desire is fullfilled")]
		public bool shouldThrowItem = false;

		[HideInInspector]
		public bool isOrder = false;
		[HideInInspector]
		public Sprite visualisationAfterPrefab;

		[Min(0), Tooltip("How long does the desire take to be fulfilled, 0 is instant")]
		public float desireFulfillTime;
	}
}
