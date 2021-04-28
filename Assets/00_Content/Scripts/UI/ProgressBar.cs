using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class ProgressBar : MonoBehaviour {
		[SerializeField] private Image progressImage;

		private void OnEnable() {
			transform.rotation = Quaternion.identity;
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		public void UpdateProgress(float progress) {
			progressImage.fillAmount = progress;
		}
	}
}
