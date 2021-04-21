using UnityEngine;
using UnityEngine.InputSystem;

namespace Cameras {
	public class StaticCameraToggle : MonoBehaviour {
		[SerializeField] private InputAction action;

		private Vector3 initialPosition;

		private void Start() {
			initialPosition = transform.position;

			action.performed += ToggleCameraMode;
			action.Enable();
		}

		private void ToggleCameraMode(InputAction.CallbackContext ctx) {
			BoundedCamera boundedCamera = GetComponent<BoundedCamera>();
			if (boundedCamera.enabled) {
				transform.position = initialPosition;
				boundedCamera.enabled = false;
			}
			else {
				boundedCamera.enabled = true;
			}
		}
	}
}