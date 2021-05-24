using TMPro;
using UnityEngine;

namespace UI {
	public class DialogueText : MonoBehaviour {
		[SerializeField] private TMP_Text textArea;

		public void TypeText(string text) {
			textArea.text = text;
		}
	}
}
