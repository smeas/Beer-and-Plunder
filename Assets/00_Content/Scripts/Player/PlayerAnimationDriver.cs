using Interactables.Instruments;
using UnityEngine;

namespace Player {
	public class PlayerAnimationDriver : MonoBehaviour {
		private Animator animator;
		private PlayerMovement playerMovement;
		private PlayerBrawling playerBrawling;
		private PlayerPickUp playerPickUp;

		public bool Celebrating {
			set => animator.SetBool("Celebrating", value);
		}

		private void Start() {
			animator = GetComponentInChildren<Animator>();
			playerMovement = GetComponent<PlayerMovement>();
			playerBrawling = GetComponent<PlayerBrawling>();
			playerPickUp = GetComponentInChildren<PlayerPickUp>();

			playerBrawling.OnAttack += HandleOnAttack;
		}

		private void Update() {
			Instrument instrument = playerPickUp.PickedUpItem as Instrument;
			bool holdingInstrument = instrument != null;

			animator.SetFloat("Speed", playerMovement.ActualSpeed / playerMovement.MaxSpeed);
			animator.SetBool("Stunned", playerBrawling.IsStunned);
			animator.SetBool("Carrying", playerPickUp.PickedUpItem != null);
			animator.SetBool("CarryingHeavy", playerPickUp.PickedUpItem != null && playerPickUp.PickedUpItem.IsHeavy);
			animator.SetBool("TwoCarrying", playerPickUp.PickedUpItem != null && playerPickUp.PickedUpItem.IsMultiCarried);
			animator.SetBool("HoldInstrument", holdingInstrument);
			animator.SetBool("PlayInstrument", holdingInstrument && instrument.IsPlaying);
		}

		private void HandleOnAttack() {
			animator.SetTrigger("Attack");
		}
	}
}