using Player;
using System;
using UnityEngine;
using Vikings;

namespace Interactables.Weapons {
	public class Axe : PickUp, IUseable {

		[SerializeField] private LayerMask pickUpLayer;
		[SerializeField] private LayerMask weaponLayer;
		[SerializeField] private WeaponData weaponData;

		public bool IsAttacking {
			get { return isAttacking; }
			set { isAttacking = value; }
		}

		public WeaponData WeaponData => weaponData;

		private Animator animator;
		private Rigidbody rb;
		private bool isAttacking;

		public override void Start() {
			base.Start();

			OnPickedUp += HandleOnPickedUp;
			OnDropped += HandleOnDropped;

			animator = GetComponent<Animator>();
		}

		private void HandleOnPickedUp(PickUp obj) {
			animator.enabled = true;
			gameObject.layer = (int)Mathf.Log(weaponLayer.value, 2);
			ObjectCollider.enabled = true;
		}
		private void HandleOnDropped(PickUp obj) {
			animator.enabled = false;
			gameObject.layer = (int)Mathf.Log(pickUpLayer.value, 2);
		}

		public void Use(GameObject user) {
			PlayerComponent player = user.GetComponent<PlayerComponent>();
			if(!player.IsStunned)
				Attack();
		}

		public void EndUse() { }

		private void Attack() {
			animator.SetTrigger("attack");
			isAttacking = true;
		}

		public void EndAttack() {
			isAttacking = false;
		}

	}
}