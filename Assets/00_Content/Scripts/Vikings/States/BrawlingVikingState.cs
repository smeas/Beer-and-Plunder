using Interactables;
using UnityEngine;
using UnityEngine.AI;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is brawling
	/// </summary>
	public class BrawlingVikingState : VikingState {
		private Table targetTable;
		private NavMeshAgent navMeshAgent;

		private const float AttackTableDuration = 5f;
		private float attackTableTimer;

		private bool IsMoving => navMeshAgent.desiredVelocity.sqrMagnitude != 0;

		public BrawlingVikingState(Viking viking, Table target = null) : base(viking) {
			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			if (target != null)
				this.targetTable = target;
			else
				SetNewTarget();

			attackTableTimer = AttackTableDuration;
		}

		public override VikingState Enter() {
			viking.bodyMeshRenderer.material = viking.brawlingMaterial;

			if (viking.CurrentChair != null) {
				viking.CurrentChair.OnVikingLeaveChair(viking);
				viking.CurrentChair = null;
			}

			return this;
		}

		public override void Exit() {
			viking.bodyMeshRenderer.material = viking.normalMaterial;
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (IsMoving) return this;

			// TODO: Damage and break table instead of timer
			attackTableTimer -= Time.deltaTime;
			if (attackTableTimer <= 0) {
				MakeTableBrawl(targetTable);
				SetNewTarget();
			}

			return this;
		}

		private void MakeTableBrawl(Table table) {
			foreach (Chair chair in table.Chairs) {
				if (chair.SittingViking != null)
					chair.SittingViking.JoinBrawl();
			}
		}

		private void SetNewTarget() {
			if (Table.AllTables.Count < 1) return;

			// Check if table is already destroyed??
			int index = Random.Range(0, Table.AllTables.Count);
			targetTable = Table.AllTables[index];

			navMeshAgent.SetDestination(targetTable.transform.position);
			attackTableTimer = AttackTableDuration;
		}

		public override bool CanInteract(GameObject player, PickUp item) {
			return true;
		}

		public override VikingState Interact(GameObject player, PickUp item) {
			viking.Stats.Reset();

			return new LeavingVikingState(viking);
		}
	}
}
