using System.Collections.Generic;
using System.Linq;
using Player;
using Rounds;
using Taverns;
using UnityEngine;
using Vikings;

namespace Interactables {
	public class Table : Interactable {
		public static List<Table> AllTables { get; } = new List<Table>();

		private float health;

		public Chair[] Chairs { get; private set; }
		public bool IsFull => Chairs.All(chair => chair.IsOccupied);
		public bool IsDestroyed => health <= 0;

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

			foreach (Chair chair in Chairs) {
				chair.Table = this;
			}

			if (RoundController.Instance != null)
				health = RoundController.Instance.CurrentDifficulty.tableHealth;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return !IsDestroyed && player.GetComponent<PlayerSteward>().Follower != null;
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

		public void Damage(float damage) {
			if (IsDestroyed) return;

			health = Mathf.Max(0, health - damage);
			if (IsDestroyed) {
				GetComponentInChildren<MeshRenderer>().enabled = false;
				if (Tavern.Instance != null)
					Tavern.Instance.TakesDamage(1);
			}
		}

		public void Repair() {
			if (RoundController.Instance != null)
				health = RoundController.Instance.CurrentDifficulty.tableHealth;

			if (Tavern.Instance != null)
				Tavern.Instance.RepairsDamage(1);

			GetComponentInChildren<MeshRenderer>().enabled = true;
		}
	}
}
