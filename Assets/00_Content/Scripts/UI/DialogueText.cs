using System.Collections;
using TMPro;
using UnityEngine;

namespace UI {
	public class DialogueText : MonoBehaviour {
		[SerializeField] private TMP_Text textArea;

		[Header("Settings")]
		[Tooltip("Characters/second")]
		[SerializeField] private float typeSpeed = 10f;

		private bool isWriting;

		public void TypeText(string text) {
			if (isWriting)
				StopAllCoroutines();

			StartCoroutine(CoTypeText(text));
		}

		private IEnumerator CoTypeText(string text) {
			textArea.enabled = true;
			isWriting = true;
			int index = 0;

			while (index < text.Length) {
				while (index < text.Length && char.IsWhiteSpace(text[index]))
					index++;

				textArea.text = text.Substring(0, index + 1);
				index++;

				yield return new WaitForSeconds(1 / typeSpeed);
			}

			isWriting = false;
		}
	}
}
