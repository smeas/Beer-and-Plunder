using DG.Tweening;
using UnityEngine;

public class DesireBubble : MonoBehaviour {
	[SerializeField] private float tweenScaleEndValue = 1.1f;
	[SerializeField] private float tweenDuration = 1f;
	[SerializeField] private Ease tweenEasing = Ease.OutQuad;

	private Tween scaleTween;

	private void OnEnable() {
		scaleTween = transform.DOScale(tweenScaleEndValue, tweenDuration).SetEase(tweenEasing).SetLoops(-1, LoopType.Yoyo);
	}

	private void OnDisable() {
		transform.DOKill();
	}

	public void SetTweenTimeScale(float timeScale) {
		scaleTween.timeScale = timeScale;
	}
}