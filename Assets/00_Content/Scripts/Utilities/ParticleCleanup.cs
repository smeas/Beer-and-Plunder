using UnityEngine;

namespace Utilities {
	public class ParticleCleanup : MonoBehaviour {
		private void Start() {
			Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
		}
	}
}