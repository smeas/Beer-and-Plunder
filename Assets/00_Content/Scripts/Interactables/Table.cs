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
		private bool isRepairing;
		private float repairTimer;
		private float repairDuration;
		private RepairTool repairTool;

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
			if (!IsDestroyed && player.GetComponent<PlayerSteward>().Follower != null)
				return true;

			if (IsDestroyed && item is RepairTool) {
				if (Tavern.Instance != null && RoundController.Instance != null &&
					Tavern.Instance.Money < RoundController.Instance.CurrentDifficulty.tableRepairCost) {
					return false;
				}

				return true;
			}

			return false;
		}

		private void Update() {
			UpdateRepairing();
		}

		public override void Interact(GameObject player, PickUp item) {
			if (IsDestroyed && item is RepairTool tool) {
				StartRepairing(player, tool);
			}
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			EndRepairing(player);
		}

		private void StartRepairing(GameObject player, RepairTool tool) {
			isRepairing = true;
			repairTool = tool;
			repairTimer = repairDuration = RoundController.Instance != null
				? RoundController.Instance.CurrentDifficulty.tableRepairTime
				: 5f;
			tool.RepairProgressImage.fillAmount = 0f;
			tool.RepairProgressCanvas.SetActive(true);
			tool.RepairProgressCanvas.transform.SetPositionAndRotation(
				transform.position + new Vector3(0, 3f, 0), Quaternion.identity);
			player.GetComponent<PlayerMovement>().CanMove = false;
		}

		private void EndRepairing(GameObject player) {
			isRepairing = false;
			repairTool.RepairProgressCanvas.SetActive(false);
			player.GetComponent<PlayerMovement>().CanMove = true;
		}

		private void UpdateRepairing() {
			if (isRepairing) {
				repairTimer -= Time.deltaTime;

				if (repairTimer <= 0) {
					isRepairing = false;
					repairTool.RepairProgressCanvas.SetActive(false);

					if (Tavern.Instance != null && RoundController.Instance != null)
						Tavern.Instance.SpendsMoney(RoundController.Instance.CurrentDifficulty.tableRepairCost);

					Repair();
				}
				else {
					repairTool.RepairProgressImage.fillAmount = 1f - repairTimer / repairDuration;
				}
			}
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

			GetComponentInChildren<MeshRenderer>().enabled = true;
		}
	}
}
