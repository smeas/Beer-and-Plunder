using UnityEngine;
using Vikings;

namespace Interactables.Kitchens {

	[CreateAssetMenu(fileName = "Food", menuName = "Game/Food")]
	public class FoodData : ScriptableObject {
		public DesireType type;
	}
}
