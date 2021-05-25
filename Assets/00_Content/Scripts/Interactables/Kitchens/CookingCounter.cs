using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables.Kitchens {
	public class CookingCounter : MonoBehaviour {
		[SerializeField] TMPro.TextMeshProUGUI counterText;

		public void SetCounter(int value) {
			counterText.text = value.ToString();
		}

		public void Enable() {
			gameObject.SetActive(true);
		}

		public void Disable() {
			gameObject.SetActive(false);
		}

	}
}
