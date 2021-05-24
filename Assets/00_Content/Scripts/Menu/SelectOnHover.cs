using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu {
	[RequireComponent(typeof(Selectable))]
	public class SelectOnHover : MonoBehaviour, IPointerEnterHandler {
		private Selectable selectable;

		private void Awake() {
			selectable = GetComponent<Selectable>();
		}

		public void OnPointerEnter(PointerEventData eventData) {
			selectable.Select();
		}
	}
}
