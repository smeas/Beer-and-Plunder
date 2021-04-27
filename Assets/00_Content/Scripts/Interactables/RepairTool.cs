using UnityEngine;
using UnityEngine.UI;

namespace Interactables {
	public class RepairTool : PickUp {
		[Space]
		[SerializeField] private GameObject repairProgressCanvas;
		[SerializeField] private Image repairProgressImage;

		public GameObject RepairProgressCanvas => repairProgressCanvas;
		public Image RepairProgressImage => repairProgressImage;
	}
}