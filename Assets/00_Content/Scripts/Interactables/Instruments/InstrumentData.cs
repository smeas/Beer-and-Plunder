using UnityEngine;

namespace Interactables.Instruments {
	[CreateAssetMenu(fileName = "Instrument", menuName = "Game/Instrument", order = 0)]
	public class InstrumentData : ScriptableObject {
		public AudioClip music;
		public float effectRadius = 10f;
		public bool playerCanMoveWhileUsing;
	}
}
