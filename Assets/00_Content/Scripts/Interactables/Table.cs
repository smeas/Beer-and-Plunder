using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vikings;

namespace Interactables {
	public class Table : Interactable {
		public static List<Table> AllTables { get; } = new List<Table>();

		public Chair[] Chairs { get; private set; }
		public bool IsFull => Chairs.All(chair => chair.IsOccupied);

		private void OnEnable() {
			AllTables.Add(this);
		}

		private void OnDisable() {
			AllTables.Remove(this);
		}

		private void Start() {
			Chairs = GetComponentsInChildren<Chair>();

			if (Chairs.Length == 0)
				Debug.LogWarning("Table has no chairs!", this);
		}

		public bool TryFindEmptyChairForViking(Viking viking, out Chair closest) {
			Vector3 vikingPosition = viking.transform.position;

			closest = null;
			float minDistance = float.PositiveInfinity;

			foreach (Chair chair in Chairs) {
				if (chair.SittingViking != null)
					continue;

				float distance = (vikingPosition - chair.SitPivot.position).sqrMagnitude;
				if (distance < minDistance) {
					minDistance = distance;
					closest = chair;
				}
			}

			return closest != null;
		}
	}
}