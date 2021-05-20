using System;
using Audio;
using Player;
using UnityEngine;

namespace Interactables.Weapons {
	public class Axe : PickUp, IUseable {

		[SerializeField] private Collider weaponCollider;
		[SerializeField] private WeaponData weaponData;
		[SerializeField] private AnimationClip referenceAnimation;
		[SerializeField] private int beginAttackFrame;

		private bool isAttacking;
		private bool isAnimating;
		private float animationTimer;

		public bool IsAttacking => isAttacking;

		public WeaponData WeaponData => weaponData;

		public event Action OnAttack;

		protected override void Start() {
			base.Start();

			OnDropped += HandleOnDropped;
		}

		private void Update() {
			if (!isAnimating) return;

			animationTimer += Time.deltaTime;
			if (!isAttacking && animationTimer >= beginAttackFrame / referenceAnimation.frameRate) {
				BeginAttack();
			}
			else if (isAttacking && animationTimer >= referenceAnimation.length) {
				EndAttack();
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (isAttacking) {
				IHittable hittable = other.GetComponentInParent<IHittable>();
				hittable?.Hit(this);
			}
		}

		private void HandleOnDropped(PickUp obj, PlayerComponent playerComponent) {
			weaponCollider.enabled = false;
		}

		public void Use(GameObject user) {
			PlayerBrawling player = user.GetComponent<PlayerBrawling>();
			if(!player.IsStunned)
				Attack();
		}

		public void EndUse() { }

		private void Attack() {
			if (isAnimating) return;

			OnAttack?.Invoke();
			isAnimating = true;
			animationTimer = 0f;
		}

		private void BeginAttack() {
			isAttacking = true;
			weaponCollider.enabled = true;
			AudioManager.PlayEffectSafe(SoundEffect.Player_SwingAxe);
		}

		private void EndAttack() {
			isAnimating = false;
			isAttacking = false;
			weaponCollider.enabled = false;
		}
	}
}