using System.Linq;
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

		private float attackTimer;

		private bool IsMoving => navMeshAgent.desiredVelocity.sqrMagnitude != 0;

		public BrawlingVikingState(Viking viking, Table targetTable) : base(viking) {
			Debug.Assert(targetTable != null, "Viking is entering a brawl with no target");
			this.targetTable = targetTable;
		}

		public override VikingState Enter() {
			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			viking.bodyMeshRenderer.material = viking.brawlingMaterial;
			viking.DismountChair();

			return this;
		}

		public override void Exit() {
			viking.bodyMeshRenderer.material = viking.normalMaterial;
			navMeshAgent.enabled = false;
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			if (IsMoving) return this;

			attackTimer -= Time.deltaTime;
			if (attackTimer <= 0) {
				targetTable.Damage(viking.Data.damage);
				attackTimer = viking.Data.attackRate;
			}

			if (targetTable.IsDestroyed) {
				MakeTableBrawl(targetTable);

				Table[] possibleTargets = Table.AllTables.Where(x => !x.IsDestroyed).ToArray();

				// The viking leaves if there are no intact tables, this should trigger a game over
				if (possibleTargets.Length == 0) return new LeavingVikingState(viking);

				int index = Random.Range(0, possibleTargets.Length);
				targetTable = possibleTargets[index];

				navMeshAgent.SetDestination(targetTable.transform.position);
				attackTimer = viking.Data.attackRate;
			}

			return this;
		}

		private void MakeTableBrawl(Table table) {
			foreach (Chair chair in table.Chairs) {
				if (chair.SittingViking != null)
					chair.SittingViking.JoinBrawl();
			}
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
