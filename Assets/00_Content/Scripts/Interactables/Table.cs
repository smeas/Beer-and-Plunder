using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using Vikings;
using Random = UnityEngine.Random;

namespace Interactables {
	public class Table : Interactable {
		public static List<Table> AllTables { get; } = new List<Table>();
		public static event Action OnTablesDestroyed;

		[SerializeField] private float maxHealth = 10f;
		[SerializeField] private float repairTime = 5f;
		[SerializeField] private Renderer bodyRenderer;
		[SerializeField] private Material[] damageStates;
		[SerializeField] private GameObject brokenTablePrefab;

		private float health;
		private bool isRepairing;
		private float repairTimer;
		private float repairDuration;
		private RepairTool repairTool;
		private GameObject brokenTable;

		public Chair[] Chairs { get; private set; }
		public bool IsFull => Chairs.All(chair => chair.IsOccupied);
		public bool IsDestroyed => health <= 0;

		public event Action Repaired;
		public event Action Destroyed;

		private void OnEnable() {
			AllTables.Add(this);
		}

		private void OnDisable() {
			AllTables.Remove(this);
		}

		private void Start() {
			if (damageStates.Length == 0)
				Debug.LogError("Table has no damage states!", this);

			Chairs = GetComponentsInChildren<Chair>();

			if (Chairs.Length == 0)
				Debug.LogWarning("Table has no chairs!", this);

			foreach (Chair chair in Chairs) {
				chair.Table = this;
			}

			health = maxHealth;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			if (!IsDestroyed && player.GetComponent<PlayerSteward>().Follower != null)
				return true;

			if (IsDestroyed && item is RepairTool)
				return true;

			return false;
		}

		public override void Interact(GameObject player, PickUp item) {
			if (IsDestroyed && item is RepairTool tool) {
				StartRepairing(player, tool);
			}
		}

		public override void CancelInteraction(GameObject player, PickUp item) {
			if (item is RepairTool tool)
				EndRepairing(player, tool);
		}

		private void StartRepairing(GameObject player, RepairTool tool) {
			tool.BeginRepairing(repairTime, transform.position + new Vector3(0, 3f, 0)); //event
			tool.RepairDone += HandleRepairDone;

			player.GetComponent<PlayerMovement>().CanMove = false;
		}

		private void EndRepairing(GameObject player, RepairTool tool) {

			tool.EndRepairing();
			tool.RepairDone -= HandleRepairDone;

			player.GetComponent<PlayerMovement>().CanMove = true;
		}

		private void HandleRepairDone() {
			Repair();
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
				bodyRenderer.gameObject.SetActive(false);
				Transform modelTransform = bodyRenderer.transform;
				brokenTable = Instantiate(brokenTablePrefab, modelTransform.position, modelTransform.rotation, transform);

				Destroyed?.Invoke();

				if (IsTablesDestroyed()) OnTablesDestroyed?.Invoke();
			}
			else {
				float healthFraction = health / maxHealth; // should never be zero here
				int damageIndex = Mathf.FloorToInt((1f - healthFraction) * damageStates.Length);
				bodyRenderer.sharedMaterial = damageStates[damageIndex];
			}
		}

		public void Repair() {
			if (!IsDestroyed) return;

			health = maxHealth;

			if (brokenTable != null)
				Destroy(brokenTable);

			bodyRenderer.gameObject.SetActive(true);
			bodyRenderer.sharedMaterial = damageStates[0];

			GetComponentInChildren<MeshRenderer>().enabled = true;
			Repaired?.Invoke();
		}

		public static Chair GetRandomEmptyChair() {
			if (AllTables.Count == 0)
				return null;

			Table[] freeTables = AllTables.Where(tbl => !tbl.IsFull && !tbl.IsDestroyed).ToArray();
			if (freeTables.Length == 0)
				return null;

			Table table = freeTables[Random.Range(0, freeTables.Length)];
			Chair[] freeChairs = table.Chairs.Where(chr => !chr.IsOccupied).ToArray();
			if (freeChairs.Length == 0)
				return null;

			return freeChairs[Random.Range(0, freeChairs.Length)];
		}

		private bool IsTablesDestroyed() {
			foreach (Table table in AllTables) {
				if (!table.IsDestroyed) return false;
			}
			return true;
		}
	}
}
