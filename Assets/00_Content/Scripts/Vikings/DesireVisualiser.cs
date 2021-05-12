using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Vikings {
	public class DesireVisualiser : MonoBehaviour {
		[SerializeField] private Color color = Color.green;
		[SerializeField] GameObject DesireBubble;
		[SerializeField] Image DesireImage;

		public void ShowNewDesire(Sprite sprite) { 

			if (sprite == null) {
				Debug.Assert(false, "Desire visualisation is null");
				return;
			}

			DesireImage.sprite = sprite;
			DesireBubble.SetActive(true);
		}

		public void HideDesire() {
			DesireBubble.SetActive(false);
		}
	}
}
