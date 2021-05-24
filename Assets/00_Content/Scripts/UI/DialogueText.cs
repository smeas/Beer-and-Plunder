using System.Collections;
using TMPro;
using UnityEngine;

namespace UI {
	public class DialogueText : MonoBehaviour {
		[SerializeField] private TMP_Text textArea;

		[Header("Settings")]
		[SerializeField] private float switchDuration = 1f;

		private float baseAlpha;

		private void Start() {
			baseAlpha = textArea.color.a;
		}

		public void TypeText(string text) {
			StopAllCoroutines();
			StartCoroutine(SwitchText(text));
		}

		private IEnumerator SwitchText(string text) {
			float halfSwitchDuration = switchDuration / 2;
			Color textAreaColor = textArea.color;

			// Fade out
			for (float time = 0; time < halfSwitchDuration; time += Time.deltaTime) {
				textAreaColor.a = Mathf.Lerp(0, baseAlpha, 1 - time / halfSwitchDuration);
				textArea.color = textAreaColor;
				yield return null;
			}

			textArea.text = text;

			// Fade in
			for (float time = 0; time < halfSwitchDuration; time += Time.deltaTime) {
				textAreaColor.a = Mathf.Lerp(0, baseAlpha, time / halfSwitchDuration);
				textArea.color = textAreaColor;
				yield return null;
			}
		}
	}
}
