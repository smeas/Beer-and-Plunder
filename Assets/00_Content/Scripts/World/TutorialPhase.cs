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
		[SerializeField] private bool setDuration;
		[SerializeField] private float duration;

		[Header("Actions")]
		[SerializeField] private GameObject[] deactivate;
		[SerializeField] private GameObject[] activate;
		[SerializeField] private bool showMoveControls;
		[SerializeField] private bool showInteractControls;
		[SerializeField] private bool showDropControls;
		[SerializeField] private GameObject highlightTarget;

		[Header("Events")]
		[SerializeField] private UnityEvent onEnter;

		public string Text => text;
		public bool SetDuration => setDuration;
		public float Duration => duration;
		public bool ShowMoveControls => showMoveControls;
		public bool ShowInteractControls => showInteractControls;
		public bool ShowDropControls => showDropControls;

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
		}

		public bool IsCorrectEvent(TutorialEvent e) {
			return e == completeOn;
		}
	}
}
