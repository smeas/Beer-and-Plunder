using Interactables;
using Interactables.Beers;
using Interactables.Instruments;
using UnityEngine;

namespace Player {
	public class PlayerAnimationDriver : MonoBehaviour {
		private static readonly int speedId = Animator.StringToHash("Speed");
		private static readonly int stunnedId = Animator.StringToHash("Stunned");
		private static readonly int carryingId = Animator.StringToHash("Carrying");
		private static readonly int carryingHeavyId = Animator.StringToHash("CarryingHeavy");
		private static readonly int twoCarryingId = Animator.StringToHash("TwoCarrying");
		private static readonly int holdInstrumentId = Animator.StringToHash("HoldInstrument");
		private static readonly int playInstrumentId = Animator.StringToHash("PlayInstrument");
		private static readonly int attackId = Animator.StringToHash("Attack");
		private static readonly int celebratingId = Animator.StringToHash("Celebrating");
		private static readonly int repairingId = Animator.StringToHash("Repairing");

		private PlayerComponent playerComponent;
		private PlayerMovement playerMovement;
		private PlayerBrawling playerBrawling;
		private PlayerPickUp playerPickUp;

		/// <summary>
		/// Play the celebration animation.
		/// </summary>
		public bool Celebrating {
			set => playerComponent.CharacterAnimator.SetBool(celebratingId, value);
		}

		private void Start() {
			playerComponent = GetComponent<PlayerComponent>();
			playerMovement = GetComponent<PlayerMovement>();
			playerBrawling = GetComponent<PlayerBrawling>();
			playerPickUp = GetComponentInChildren<PlayerPickUp>();

			playerBrawling.OnAttack += HandleOnAttack;
		}

		private void Update() {
			PickUp carriedItem = playerPickUp.PickedUpItem;
			bool carrying = carriedItem != null;

			Instrument instrument = playerPickUp.PickedUpItem as Instrument;
			bool holdingInstrument = instrument != null;

			RepairTool repairTool = playerPickUp.PickedUpItem as RepairTool;
			bool holdingRepairTool = repairTool != null;

			playerComponent.CharacterAnimator.SetFloat(speedId, playerMovement.ActualSpeed / playerMovement.MaxSpeed);
			playerComponent.CharacterAnimator.SetBool(stunnedId, playerBrawling.IsStunned);

			playerComponent.CharacterAnimator.SetBool(carryingId, carrying);
			playerComponent.CharacterAnimator.SetBool(carryingHeavyId, carrying && carriedItem.IsHeavy);
			playerComponent.CharacterAnimator.SetBool(twoCarryingId, carrying && carriedItem is BeerBarrel);

			playerComponent.CharacterAnimator.SetBool(holdInstrumentId, holdingInstrument);
			playerComponent.CharacterAnimator.SetBool(playInstrumentId, holdingInstrument && instrument.IsPlaying);
			playerComponent.CharacterAnimator.SetBool(repairingId, holdingRepairTool && repairTool.IsRepairing);

		}

		private void HandleOnAttack() {
			playerComponent.CharacterAnimator.SetTrigger(attackId);
		}
	}
}