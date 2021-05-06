using Audio;
using Player;
using UnityEngine;

namespace Interactables.Weapons {
	public class Axe : PickUp, IUseable {

		[SerializeField] private Collider weaponCollider;
		[SerializeField] private WeaponData weaponData;

		public bool IsAttacking {
			get { return isAttacking; }
			set { isAttacking = value; }
		}

		public WeaponData WeaponData => weaponData;

		private Animator animator;
		private bool isAttacking;

		protected override void Start() {
			base.Start();

			OnPickedUp += HandleOnPickedUp;
			OnDropped += HandleOnDropped;

			animator = GetComponent<Animator>();
		}

		private void HandleOnPickedUp(PickUp obj) {
			animator.enabled = true;
			weaponCollider.enabled = true;
		}
		private void HandleOnDropped(PickUp obj) {
			animator.enabled = false;
			weaponCollider.enabled = false;
		}

		public void Use(GameObject user) {
			PlayerBrawling player = user.GetComponent<PlayerBrawling>();
			if(!player.IsStunned)
				Attack();
		}

		public void EndUse() { }

		private void Attack() {
			if (isAttacking) return;

			animator.SetTrigger("attack");
			isAttacking = true;
		}

		//Run from AnimationEvent
		public void EndAttack() {
			isAttacking = false;
		}

		// Run from AnimationEvent
		private void PlaySwingSound() {
			AudioManager.PlayEffectSafe(SoundEffect.Player_SwingAxe);
		}
	}
}