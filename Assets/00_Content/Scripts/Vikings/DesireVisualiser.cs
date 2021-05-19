using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;

namespace Vikings {
	public class DesireVisualiser : MonoBehaviour {
		[SerializeField] DesireBubble desireBubble;
		[SerializeField] Image desireImage;

		[Header("Settings")]
		[SerializeField] Color lowDesire;
		[SerializeField] Color highDesire;
		[SerializeField] float tweenMaxSpeed = 4f;

		private Image desireBubbleImage;

		public bool IsShowing { get; private set; }

		private void Start() {
			desireBubbleImage = desireBubble.gameObject.GetComponent<Image>();
		}

		public void ShowNewDesire(Sprite sprite) {
			if (sprite == null) {
				Debug.Assert(false, "Desire visualisation is null");
				return;
			}

			desireImage.sprite = sprite;
			desireBubble.gameObject.SetActive(true);
			IsShowing = true;
		}

		public void HideDesire() {
			desireBubble.gameObject.SetActive(false);
			IsShowing = false;
		}

		public void SetDesireColor(float remappedMood) {
			desireBubbleImage.color = Color.Lerp(highDesire, lowDesire, remappedMood);
		}

		public void SetTweenSpeed(float remappedMood) {
			float newTimeScale = MathX.RemapClamped(1 - remappedMood, 0, 1, 1, tweenMaxSpeed);
			desireBubble.SetTweenTimeScale(newTimeScale);
		}
	}
}
