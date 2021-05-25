using System.Linq;
using Audio;
using DG.Tweening;
using Interactables;
using Interactables.Weapons;
using Player;
using UnityEngine;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is brawling
	/// </summary>
	public class BrawlingVikingState : VikingState {
		private Table targetTable;
		private Viking vikingTarget;
		private PlayerComponent playerTarget;
		private BrawlType brawlType;

		private float attackTimer;
		private bool isDismountingChair;
		private bool lastIsMoving;
		private bool isBrawlingCurrentTable;

		private bool IsMoving =>  viking.NavMeshAgent.pathPending || viking.NavMeshAgent.desiredVelocity.sqrMagnitude != 0;

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
			brawlType = BrawlType.PlayerBrawl;
		}

		public override VikingState Enter() {
			viking.animationDriver.Brawl = true;
			AudioManager.PlayEffectSafe(SoundEffect.Viking_Brawling_Angry);
			viking.angryParticleSystem.Play();

			if (viking.CurrentChair != null) {
				if (brawlType == BrawlType.TableBrawl && targetTable.Chairs.Contains(viking.CurrentChair))
					isBrawlingCurrentTable = true;

				viking.DismountChair();
				isDismountingChair = true;
			}
			else {
				OnChairDismounted();
			}

			return this;
		}

		public override void Exit() {
			viking.animationDriver.Brawl = false;
			viking.animationDriver.TableBrawl = false;

			viking.NavMeshAgent.enabled = false;
			viking.angryParticleSystem.Stop();
		}

		private void OnChairDismounted() {
			isDismountingChair = false;
			viking.NavMeshAgent.enabled = true;

			if (brawlType == BrawlType.TableBrawl) {
				if (isBrawlingCurrentTable) {
					viking.NavMeshAgent.enabled = false;
					LookAtTable();
				}
				else {
					viking.NavMeshAgent.SetDestination(targetTable.transform.position);
					attackTimer = viking.Data.attackRate;
				}
			}
		}

		public override VikingState HandleOnHit(Axe axe, Viking viking) {
			viking.Stats.TakeBrawlDamage(axe.WeaponData.brawlDamage);

			if(brawlType == BrawlType.TableBrawl) {
				return new BrawlingVikingState(viking, axe.GetComponentInParent<PlayerComponent>());
			}

			if (viking.Stats.BrawlHealth <= 0) {

				return new LeavingVikingState(viking);
			}
			return this;
		}

		public override VikingState Update() {
			if (isDismountingChair) {
				if (!viking.animationDriver.IsSitting)
					OnChairDismounted();

				return this;
			}

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
			// When we stop moving, make sure we're looking at the table.
			if (lastIsMoving != (lastIsMoving = IsMoving) && !IsMoving) {
				viking.NavMeshAgent.enabled = false;
				LookAtTable();
			}

			if (targetTable.IsDestroyed) {
				MakeTableBrawl(targetTable);

				Table[] possibleTargets = Table.AllTables.Where(x => !x.IsDestroyed).ToArray();

				// The viking targets a player if there are no intact tables, otherwise leaves
				// This is used in the tutorial but shouldn't happen in a normal game (should probably be game over)
				if (possibleTargets.Length == 0) {
					if (PlayerManager.Instance != null && PlayerManager.Instance.NumPlayers > 0) {
						PlayerComponent randomPlayer = PlayerManager.Instance.Players[Random.Range(0, PlayerManager.Instance.NumPlayers)];
						return new BrawlingVikingState(viking, randomPlayer);
					}

					return new LeavingVikingState(viking);
				}

				int index = Random.Range(0, possibleTargets.Length);
				targetTable = possibleTargets[index];
				isBrawlingCurrentTable = false;

				viking.NavMeshAgent.enabled = true;
				viking.NavMeshAgent.SetDestination(targetTable.transform.position);
				attackTimer = viking.Data.attackRate;
			}

			viking.animationDriver.TableBrawl = !IsMoving;

			if (IsMoving) return this;

			attackTimer -= Time.deltaTime;
			if (attackTimer <= 0) {
				targetTable.Damage(viking.Data.damage);
				attackTimer = viking.Data.attackRate;
			}

			return this;
		}

		private void LookAtTable() {
			Vector3 tableDirection = (targetTable.transform.position - viking.transform.position).normalized;
			viking.transform.DORotateQuaternion(Quaternion.LookRotation(tableDirection), 0.2f);

			const float targetDistanceFromTable = 1.8f;
			float distanceFromTable = Vector3.Distance(viking.transform.position, targetTable.transform.position);
			if (distanceFromTable > targetDistanceFromTable) {
				Vector3 targetPosition = targetTable.transform.position - tableDirection * targetDistanceFromTable;
				viking.transform.DOMove(targetPosition, (distanceFromTable - targetDistanceFromTable) / viking.NavMeshAgent.speed);
			}
		}

		private VikingState DoVikingBrawl() {
			Debug.Log("Fighting another viking...");
			return this;
		}

		private VikingState DoPlayerBrawl() {

			if ((viking.NavMeshAgent.transform.position - playerTarget.transform.position).sqrMagnitude <
				viking.Data.attackTriggerDistance * viking.Data.attackTriggerDistance
				&& !viking.IsAttacking) {
				viking.MakeAttack();
				return this;
			}

			if (!viking.IsAttacked) {
				viking.NavMeshAgent.enabled = true;
				viking.NavMeshAgent.SetDestination(playerTarget.transform.position);
			}

			return this;
		}

		private void MakeTableBrawl(Table table) {
			foreach (Chair chair in table.Chairs) {
				if (chair.SittingViking != null)
					chair.SittingViking.JoinBrawl();
			}
		}
	}
}
