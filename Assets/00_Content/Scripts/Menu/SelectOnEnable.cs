using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu {
	public class SelectOnEnable : MonoBehaviour {
		[SerializeField] private Selectable selection;

		private void OnEnable() {
			StartCoroutine(CoNudgeSelection(selection.gameObject));
		}

		private static IEnumerator CoNudgeSelection(GameObject selection) {
			// Wait for the next frame to let things initialize.
			yield return null;

			// HACK: Make to make the selection visible.
			MoveSelection(MoveDirection.Up);
			MoveSelection(MoveDirection.Down);
			MoveSelection(MoveDirection.Left);
			MoveSelection(MoveDirection.Right);
			EventSystem.current.SetSelectedGameObject(selection);
		}

		private static void MoveSelection(MoveDirection dir) {
			EventSystem eventSystem = EventSystem.current;
			AxisEventData data = new AxisEventData(eventSystem) {
				moveDir = dir,
				selectedObject = eventSystem.currentSelectedGameObject,
			};

			ExecuteEvents.Execute(data.selectedObject, data, ExecuteEvents.moveHandler);
		}
	}
}
