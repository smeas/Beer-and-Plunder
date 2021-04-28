using System.Collections;
using System.Linq;
using Interactables;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is brawling
	/// </summary>
	public class BrawlingVikingState : VikingState {
		private Table targetTable;
		private Viking vikingTarget;
		private PlayerComponent playerTarget;
		private NavMeshAgent navMeshAgent;
		private BrawlType brawlType;

		private bool isAttacking;
		private float attackTimer;

		private bool IsMoving => navMeshAgent.desiredVelocity.sqrMagnitude != 0;

		public BrawlingVikingState(Viking viking, Table targetTable) : base(viking) {
			Debug.Assert(targetTable != null, "Viking is entering a tableBrawl with no target");
			this.targetTable = targetTable;
			brawlType = BrawlType.TableBrawl;
		}

		public BrawlingVikingState(Viking viking, Viking vikingTarget) : base(viking) {
			Debug.Assert(vikingTarget != null, "Viking is entering a vikingBrawl with no target");
			this.vikingTarget = vikingTarget;
			brawlType = BrawlType.VikingBrawl;
		}

		public BrawlingVikingState(Viking viking, PlayerComponent playerTarget) : base(viking) {
			Debug.Assert(playerTarget != null, "Viking is entering a playerBrawl with no target");
			this.playerTarget = playerTarget;
			viking.IsSeated = false;
			brawlType = BrawlType.PlayerBrawl;
		}

		public override VikingState Enter() {
			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;

			viking.bodyMeshRenderer.material = viking.brawlingMaterial;

			if(brawlType == BrawlType.TableBrawl)
				viking.DismountChair();

			return this;
		}

		public override void Exit() {
			viking.bodyMeshRenderer.material = viking.normalMaterial;
			navMeshAgent.enabled = false;
			viking.IsAttacking = false;
		}

		public override VikingState Update() {
			viking.Stats.Decline();

			switch (brawlType) {
				case BrawlType.TableBrawl:
					return DoTableBrawl();
				case BrawlType.VikingBrawl:
					return DoVikingBrawl();
				case BrawlType.PlayerBrawl:
					return DoPlayerBrawl();
				default:
					return this;
			}
		}

		private VikingState DoTableBrawl() {
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

		private VikingState DoVikingBrawl() {
			Debug.Log("Fighting other viking...");
			return this;
		}

		private VikingState DoPlayerBrawl() {

			if ((navMeshAgent.transform.position - playerTarget.transform.position).sqrMagnitude < viking.Data.attackTriggerDistance && !viking.IsAttacking) {
				viking.MakePlayerAttack(playerTarget);
				return this;
			}

			navMeshAgent.SetDestination(playerTarget.transform.position);
			
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
