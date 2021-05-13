using UnityEngine;

namespace Utilities {
	public class BillboardLookAt : MonoBehaviour {
		Camera mainCamera;

		private void Start() {
			mainCamera = Camera.main;
		}

		private void Update() {
			transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
		}
	}
}
