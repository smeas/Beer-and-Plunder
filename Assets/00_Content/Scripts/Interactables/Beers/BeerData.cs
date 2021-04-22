using UnityEngine;
using Vikings;

namespace Interactables.Beers {

	[CreateAssetMenu(fileName = "Beer", menuName = "Game/Beer")]
	public class BeerData : ScriptableObject {
		public DesireType type;
		public int cost;
	}
}
