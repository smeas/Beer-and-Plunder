using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class ProgressBar : MonoBehaviour {
		[SerializeField] private Image progressImage;
		[SerializeField] private Vector3 rotation;

	#if UNITY_EDITOR
		private void OnValidate() {
			transform.eulerAngles = rotation;
		}
	#endif

		private void OnEnable() {
			transform.eulerAngles = rotation;
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
