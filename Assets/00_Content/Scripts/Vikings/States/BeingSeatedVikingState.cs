using Interactables;
using Player;
using UnityEngine;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is being seated by a player - the player is leading the viking to their chair.
	/// </summary>
	public class BeingSeatedVikingState : VikingState {
		private const float RecalculatePathDelay = 0.5f;

		private readonly GameObject player;
		private float pathTimer;
		private float followingTimer;

		public BeingSeatedVikingState(Viking viking, GameObject player) : base(viking) {
			this.player = player;
		}

		public override VikingState Enter() {
			viking.NavMeshAgent.enabled = true;
			_ = viking.NavMeshAgent.SetDestination(player.transform.position);
			pathTimer = RecalculatePathDelay;

			Color playerColor = player.GetComponent<PlayerComponent>().PlayerColor;
			Material material = viking.followingRingRenderer.material;
			material.color = new Color(playerColor.r, playerColor.g, playerColor.b, material.color.a);
			viking.followingRingRenderer.enabled = true;

			return this;
		}

		public override void Exit() {
			viking.NavMeshAgent.enabled = false;
			player.GetComponent<PlayerSteward>().EndSeatingViking(viking);

			viking.followingRingRenderer.enabled = false;
		}

		public override VikingState Update() {
			pathTimer -= Time.deltaTime;
			if (viking.NavMeshAgent.enabled && pathTimer <= 0) {
				pathTimer = RecalculatePathDelay;
				_ = viking.NavMeshAgent.SetDestination(player.transform.position);
			}

			followingTimer += Time.deltaTime;
			if (!viking.Data.FollowPlayerIndefinitely && followingTimer >= viking.Data.MaxFollowingDuration) {
				Chair chair = Table.GetRandomEmptyChair();

				if (chair != null)
					return TakeSeat(chair);

				return new LeavingVikingState(viking);
			}

			return this;
		}

		public override VikingState TakeSeat(Chair chair) {
			// Reserve the chair
			viking.CurrentChair = chair;
			chair.OnVikingTakeChair(viking);

			viking.TakingSeat?.Invoke();
			return new TakingSeatVikingState(viking, chair);
		}
	}
}
