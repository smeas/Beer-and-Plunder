using DG.Tweening;
using Interactables;
using UnityEngine;

namespace Vikings {
	public class VikingAnimationDriver : MonoBehaviour {
		private static readonly int sittingId = Animator.StringToHash("Sitting");
		private static readonly int speedId = Animator.StringToHash("Speed");
		private static readonly int gettingAngryId = Animator.StringToHash("GettingAngry");
		private static readonly int drinkingId = Animator.StringToHash("Drinking");
		private static readonly int eatingId = Animator.StringToHash("Eating");
		private static readonly int brawlId = Animator.StringToHash("Brawl");
		private static readonly int tableBrawlId = Animator.StringToHash("TableBrawl");
		private static readonly int happyId = Animator.StringToHash("Happy");
		private static readonly int attackId = Animator.StringToHash("Attack");
		private static readonly int throwId = Animator.StringToHash("Throw");
		private static readonly int requestId = Animator.StringToHash("Request");
		private static readonly int gettingAngryEffectId = Animator.StringToHash("GettingAngryEffect");

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
		public bool IsThrowing { get; private set; }

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
					animator.SetBool(sittingId, true);
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

			animator.SetBool(sittingId, false);

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
					animator.SetBool(sittingId, false);
				})
				.Append(vikingTransform.DOMove(dismountPosition, referenceUnsitAnimation.length));
		}

		public void InterruptEndSitting() {
			if (!isUnsitting) return;

			IsSitting = true;
			sitTween = null;
			isUnsitting = false;

			animator.SetBool(sittingId, true);

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
			viking.highlightPivot.position = sitTransform.position;
		}

		private void DetachSitTransform() {
			if (!sitTransformAttached) return;

			// Restore old parent
			vikingTransform.SetParent(sitTransform.parent);
			sitTransformAttached = false;
			viking.highlightPivot.localPosition = Vector3.zero;
		}

		#endregion

		public float Speed { set => animator.SetFloat(speedId, value); }
		public bool GettingAngry { set => animator.SetBool(gettingAngryId, value); }
		public bool Drinking { set => animator.SetBool(drinkingId, value); }
		public bool Eating { set => animator.SetBool(eatingId, value); }
		public bool Brawl { set => animator.SetBool(brawlId, value); }
		public bool TableBrawl { set => animator.SetBool(tableBrawlId, value); }

		public void TriggerHappy() {
			animator.SetTrigger(happyId);
			IsPlayingHappyAnimation = true;
		}

		public void TriggerAttack() {
			animator.SetTrigger(attackId);
			IsPlayingAttackAnimation = true;
		}

		public void TriggerThrow() {
			animator.SetTrigger(throwId);
			IsThrowing = true;
		}

		public void TriggerRequest() => animator.SetTrigger(requestId);
		public void TriggerGettingAngryEffect() => animator.SetTrigger(gettingAngryEffectId);

		#region Animation Events

		private void OnSitEnd() {
			if (!isBeginningSitting) return;

			sitAnimationDone = true;
		}

		private void OnUnsitEnd() {
			if (!isUnsitting) return;

			OnEndSittingCompleted();
		}

		private void OnHappyEnd() => IsPlayingHappyAnimation = false;

		private void OnAttackEnd() => IsPlayingAttackAnimation = false;

		private void OnThrowEnd() => IsThrowing = false;

		private void OnHitTable() {
			// TODO: Play table hit sound here.
		}

		#endregion
	}
}
