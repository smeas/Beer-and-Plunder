using Interactables;
using Interactables.Instruments;
using UnityEngine;

namespace Player {
	public class PlayerAnimationDriver : MonoBehaviour {
		private PlayerComponent playerComponent;
		private PlayerMovement playerMovement;
		private PlayerBrawling playerBrawling;
		private PlayerPickUp playerPickUp;

		/// <summary>
		/// Play the celebration animation.
		/// </summary>
		public bool Celebrating {
			set => playerComponent.CharacterAnimator.SetBool("Celebrating", value);
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

			playerComponent.CharacterAnimator.SetFloat("Speed", playerMovement.ActualSpeed / playerMovement.MaxSpeed);
			playerComponent.CharacterAnimator.SetBool("Stunned", playerBrawling.IsStunned);

			playerComponent.CharacterAnimator.SetBool("Carrying", carrying);
			playerComponent.CharacterAnimator.SetBool("CarryingHeavy", carrying && carriedItem.IsHeavy);
			playerComponent.CharacterAnimator.SetBool("TwoCarrying", carrying && carriedItem.IsMultiCarried);

			playerComponent.CharacterAnimator.SetBool("HoldInstrument", holdingInstrument);
			playerComponent.CharacterAnimator.SetBool("PlayInstrument", holdingInstrument && instrument.IsPlaying);
		}

		private void HandleOnAttack() {
			playerComponent.CharacterAnimator.SetTrigger("Attack");
		}
	}
}