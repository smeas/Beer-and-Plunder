using Interactables;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Vikings.States {
	/// <summary>
	/// State for when the viking is being seated by a player - the player is leading the viking to their chair.
	/// </summary>
	public class BeingSeatedVikingState : VikingState {
		private const float RecalculatePathDelay = 0.5f;

		private readonly GameObject player;
		private GameObject highlight;
		private NavMeshAgent navMeshAgent;
		private float pathTimer;

		public BeingSeatedVikingState(Viking viking, GameObject player) : base(viking) {
			this.player = player;
		}

		public override VikingState Enter() {
			highlight = Object.Instantiate(viking.beingSeatedHighlightPrefab, viking.transform);
			highlight.transform.localPosition = Vector3.up * 2;

			navMeshAgent = viking.GetComponent<NavMeshAgent>();
			navMeshAgent.enabled = true;
			_ = navMeshAgent.SetDestination(player.transform.position);
			pathTimer = RecalculatePathDelay;

			return this;
		}

		public override void Exit() {
			Object.Destroy(highlight);
			navMeshAgent.enabled = false;
		}

		public override VikingState Update() {
			pathTimer -= Time.deltaTime;
			if (pathTimer <= 0) {
				pathTimer = RecalculatePathDelay;
				_ = navMeshAgent.SetDestination(player.transform.position);
			}

			return this;
		}

		public override VikingState TakeSeat(Chair chair) {
			player.GetComponent<PlayerSteward>().EndSeatingViking(viking);

			// Reserve the chair
			viking.CurrentChair = chair;
			chair.OnVikingTakeChair(viking);

			return new TakingSeatVikingState(viking, chair);
		}
	}
}