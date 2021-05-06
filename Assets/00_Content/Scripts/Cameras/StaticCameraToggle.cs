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

		private void OnDestroy() {
			action.performed -= ToggleCameraMode;
		}

		private void ToggleCameraMode(InputAction.CallbackContext ctx) {
			FollowingCamera boundedCamera = GetComponent<FollowingCamera>();
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