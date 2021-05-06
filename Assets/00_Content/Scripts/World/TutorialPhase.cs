using System;
using UnityEngine;
using UnityEngine.Events;

namespace World {
	[Serializable]
	public class TutorialPhase {
		[SerializeField] private string name;
		[TextArea]
		[SerializeField] private string text;
		[SerializeField] private TutorialEvent completeOn;

		[Header("Actions")]
		[SerializeField] private GameObject[] deactivate;
		[SerializeField] private GameObject[] activate;
		[SerializeField] private GameObject highlightTarget;

		[Header("Events")]
		[SerializeField] private UnityEvent onEnter;

		public string Text => text;

		public void Enter(GameObject highlightObject) {
			foreach (GameObject gameObject in deactivate) {
				gameObject.SetActive(false);
			}

			foreach (GameObject gameObject in activate) {
				gameObject.SetActive(true);
			}

			if (highlightTarget != null) {
				MoveHighlight(highlightObject, highlightTarget);
			}
			else {
				highlightObject.SetActive(false);
				highlightObject.transform.parent = null;
			}

			onEnter?.Invoke();
		}

		private void MoveHighlight(GameObject highlightObject, GameObject target) {
			highlightObject.SetActive(true);
			highlightObject.transform.position = target.transform.position + new Vector3(0, 4.5f, 0);
			highlightObject.transform.parent = target.transform;
		}

		public bool OnEvent(TutorialEvent e) {
			return e == completeOn;
		}
	}
}
