using TMPro;
using UnityEngine;

namespace Rounds {
	public class PlayerReadyCard : MonoBehaviour {
		[SerializeField] private TMP_Text nameText;
		[SerializeField] private GameObject readyText;

		private bool ready;

		private void Start() {
			ready = false;
			readyText.SetActive(false);
		}

		public string Name {
			set => nameText.text = value;
		}

		public bool Ready {
			get => ready;
			set {
				ready = value;
				readyText.SetActive(value);
			}
		}

		public void ToggleReady() {
			Ready = !Ready;
		}
	}
}