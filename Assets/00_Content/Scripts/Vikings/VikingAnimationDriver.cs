using DG.Tweening;
using Interactables;
using UnityEngine;

namespace Vikings {
	public class VikingAnimationDriver : MonoBehaviour {
		[SerializeField] private AnimationClip referenceSitAnimation;
		[SerializeField] private AnimationClip referenceUnsitAnimation;
		[SerializeField] private float sitTurnDuration = 0.5f;

		private Animator animator;
		private Viking viking;
		private Transform vikingTransform;

		private bool isBeginningSitting;
		private bool isUnsitting;
		private bool sitAnimationDone;
		private bool firstSitTweenDone;
		private Vector3 tablePosition;
		private Tween sitTween;

		private Transform sitTransform;
		private bool sitTransformAttached;

		private Vector3 lastPosition;

		public bool IsSitting { get; private set; }
		public bool IsPlayingHappyAnimation { get; private set; }
		public bool IsPlayingAttackAnimation { get; private set; }

		private void Start() {
			animator = GetComponent<Animator>();
			viking = GetComponentInParent<Viking>();
			vikingTransform = viking.transform;

			sitTransform = new GameObject("Sit Pivot Helper").transform;
			sitTransform.SetParent(vikingTransform);
		}

		private void Update() {
			if (isBeginningSitting) {
				if (firstSitTweenDone && sitAnimationDone) {
					BeginSittingStage2();
				}
			}

			Vector3 currentPosition = vikingTransform.position;
			Speed = (currentPosition - lastPosition).magnitude / Time.deltaTime;
			lastPosition = currentPosition;
		}

		#region Sit Stuff

		public void BeginSitting(Vector3 sitPoint, Vector3 tablePos) {
			if (isBeginningSitting) return;
			if (isUnsitting) {
				InterruptEndSitting();
				return;
			}
			if (IsSitting) return;

			IsSitting = false;
			sitAnimationDone = false;
			isBeginningSitting = true;

			tablePosition = tablePos;

			// Calculate a rotation facing away from the chair
			Vector3 chairDirection = sitPoint - vikingTransform.position;
			chairDirection.y = 0;
			chairDirection.Normalize();
			Quaternion preSitRotation = Quaternion.LookRotation(-chairDirection, Vector3.up);

			sitTween = DOTween.Sequence()
				.Append(vikingTransform.DORotateQuaternion(preSitRotation, sitTurnDuration))
				.AppendCallback(() => {
					AttachSitTransform();
					animator.SetBool("Sitting", true);
				})
				.Append(sitTransform.DOMove(sitPoint, referenceSitAnimation.length))
				.AppendCallback(() => firstSitTweenDone = true);
		}

		private void BeginSittingStage2() {
			firstSitTweenDone = false;
			sitAnimationDone = false;

			// Rotate to face the table
			Vector3 tableDirection = (tablePosition - sitTransform.position).normalized;
			Quaternion postSitRotation = Quaternion.LookRotation(tableDirection, Vector3.up);
			sitTween = sitTransform
				.DORotateQuaternion(postSitRotation, sitTurnDuration)
				.OnComplete(OnBeginSittingCompleted);
		}

		public void InterruptBeginSitting() {
			if (!isBeginningSitting) return;

			sitTween.Kill();

			IsSitting = false;
			sitTween = null;
			sitAnimationDone = false;
			firstSitTweenDone = false;
			isBeginningSitting = false;

			animator.SetBool("Sitting", false);

			DetachSitTransform();
		}

		private void OnBeginSittingCompleted() {
			IsSitting = true;
			sitTween = null;
			sitAnimationDone = false;
			firstSitTweenDone = false;
			isBeginningSitting = false;
		}

		public void EndSitting() {
			if (isUnsitting) return;
			if (isBeginningSitting) {
				InterruptBeginSitting();
				return;
			}
			if (!IsSitting) return;

			isUnsitting = true;

			Chair chair = viking.CurrentChair;
			Vector3 dismountPosition = chair.DismountPoint.position;
			Vector3 dismountDirection = (dismountPosition - sitTransform.position).normalized;
			Quaternion dismountRotation = Quaternion.LookRotation(dismountDirection, Vector3.up);

			sitTween = DOTween.Sequence()
				.Append(sitTransform.DORotateQuaternion(dismountRotation, sitTurnDuration))
				.AppendCallback(() => {
					DetachSitTransform();
					animator.SetBool("Sitting", false);
				})
				.Append(vikingTransform.DOMove(dismountPosition, referenceUnsitAnimation.length));
		}

		public void InterruptEndSitting() {
			if (!isUnsitting) return;

			IsSitting = true;
			sitTween = null;
			isUnsitting = false;

			animator.SetBool("Sitting", true);

			AttachSitTransform();
		}

		private void OnEndSittingCompleted() {
			IsSitting = false;
			sitTween = null;
			isUnsitting = false;
		}


		private void AttachSitTransform() {
			if (sitTransformAttached) return;

			// Align with sit pivot
			sitTransform.SetParent(vikingTransform);
			sitTransform.localPosition = viking.pivotWhenSitting.localPosition;
			sitTransform.localRotation = Quaternion.identity;

			// Make it parent to the viking
			sitTransform.SetParent(vikingTransform.parent);
			vikingTransform.SetParent(sitTransform);

			sitTransformAttached = true;
		}

		private void DetachSitTransform() {
			if (!sitTransformAttached) return;

			// Restore old parent
			vikingTransform.SetParent(sitTransform.parent);
			sitTransformAttached = false;
		}

		#endregion

		public float Speed { set => animator.SetFloat("Speed", value); }
		public bool GettingAngry { set => animator.SetBool("GettingAngry", value); }
		public bool Drinking { set => animator.SetBool("Drinking", value); }
		public bool Eating { set => animator.SetBool("Eating", value); }
		public bool Brawl { set => animator.SetBool("Brawl", value); }
		public bool TableBrawl { set => animator.SetBool("TableBrawl", value); }

		public void TriggerHappy() {
			animator.SetTrigger("Happy");
			IsPlayingHappyAnimation = true;
		}

		public void TriggerThrow() => animator.SetTrigger("Throw");
		public void TriggerRequest() => animator.SetTrigger("Request");
		public void TriggerGettingAngryEffect() => animator.SetTrigger("GettingAngryEffect");
		public void TriggerAttack() {
			animator.SetTrigger("Attack");
			IsPlayingAttackAnimation = true;
		}

		#region Animation Events

		private void OnSitEnd() {
			if (!isBeginningSitting) return;

			sitAnimationDone = true;
		}

		private void OnUnsitEnd() {
			if (!isUnsitting) return;

			OnEndSittingCompleted();
		}

		private void OnHappyEnd() {
			IsPlayingHappyAnimation = false;
		}

		private void OnAttackEnd() {
			IsPlayingAttackAnimation = false;
		}

		#endregion
	}
}