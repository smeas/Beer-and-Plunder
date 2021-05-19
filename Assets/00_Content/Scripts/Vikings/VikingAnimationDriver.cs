using DG.Tweening;
using UnityEngine;

namespace Vikings {
	public class VikingAnimationDriver : MonoBehaviour {
		[SerializeField] private AnimationClip referenceSitAnimation;
		[SerializeField] private float sitTurnDuration = 0.5f;

		private Animator animator;
		private Viking viking;
		private Transform vikingTransform;

		private bool isPerformingSitAction;
		private bool sitAnimationDone;
		private bool firstSitTweenDone;
		private Vector3 tablePosition;
		private Tween sitTween;

		private Transform sitTransform;
		private bool sitTransformAttached;

		public bool IsSitting { get; private set; }

		private void Start() {
			animator = GetComponent<Animator>();
			viking = GetComponentInParent<Viking>();
			vikingTransform = viking.transform;

			sitTransform = new GameObject("Sit Pivot Helper").transform;
			sitTransform.SetParent(vikingTransform);
		}

		private void Update() {
			if (isPerformingSitAction) {
				if (firstSitTweenDone && sitAnimationDone) {
					BeginSittingStage2();
				}
			}
		}

		public void BeginSitting(Vector3 sitPoint, Vector3 tablePos) {
			IsSitting = false;
			sitAnimationDone = false;
			isPerformingSitAction = true;

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

		public void InterruptSeating() {
			if (!isPerformingSitAction) return;

			sitTween.Kill();

			IsSitting = true;
			sitTween = null;
			sitAnimationDone = false;
			firstSitTweenDone = false;
			isPerformingSitAction = false;

			animator.SetBool("Sitting", false);

			DetachSitTransform();
		}

		private void OnBeginSittingCompleted() {
			IsSitting = true;
			sitTween = null;
			sitAnimationDone = false;
			firstSitTweenDone = false;
			isPerformingSitAction = false;
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

		#region Animation Events

		private void OnSitEnd() {
			if (!isPerformingSitAction) return;

			sitAnimationDone = true;
		}

		#endregion
	}
}