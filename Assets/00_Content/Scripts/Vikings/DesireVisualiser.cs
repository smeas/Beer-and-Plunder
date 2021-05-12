using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Vikings {
	public class DesireVisualiser : MonoBehaviour {
		[SerializeField] private Color color = Color.green;
		[SerializeField] GameObject desireBubble;
		[SerializeField] Image desireImage;

		[SerializeField] Color lowDesire;
		[SerializeField] Color mediumDesire;
		[SerializeField] Color highDesire;

		private Image desireBubbleImage;

		private void Start() {
			desireBubbleImage = desireBubble.GetComponent<Image>();
		}

		public void ShowNewDesire(Sprite sprite) { 

			if (sprite == null) {
				Debug.Assert(false, "Desire visualisation is null");
				return;
			}

			desireImage.sprite = sprite;
			desireBubble.SetActive(true);
		}

		public void HideDesire() {
			desireBubble.SetActive(false);
		}

		//TODO -> Expose these values in inspector
		public void SetDesireColor(float mood) {
			if (mood > 40 && mood < 50)
				desireBubbleImage.color = lowDesire;
			else if (mood > 25 && mood < 40)
				desireBubbleImage.color = mediumDesire;
			else if (mood < 25)
				desireBubbleImage.color = highDesire;
			else
				desireBubbleImage.color = Color.white;
		}
	}
}
