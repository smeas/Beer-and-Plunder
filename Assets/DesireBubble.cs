using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DesireBubble : MonoBehaviour
{
	[SerializeField] private float tweenScaleEndValue = 1.1f;
	[SerializeField] private float tweenDuration = 1f;
	[SerializeField] private Ease tweenEasing = Ease.OutQuad;

	private void OnEnable() {
		transform.DOScale(tweenScaleEndValue, tweenDuration).SetEase(tweenEasing).SetLoops(-1, LoopType.Yoyo);
	}
}
