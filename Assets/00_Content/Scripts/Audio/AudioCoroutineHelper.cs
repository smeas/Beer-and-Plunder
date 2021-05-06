using System.Collections;
using UnityEngine;

namespace Audio {
	public class AudioCoroutineHelper : MonoBehaviour {
		private static AudioCoroutineHelper instance;

		public static AudioCoroutineHelper Instance {
			get {
				if (instance == null) {
					instance = new GameObject("AudioCoroutineHelper").AddComponent<AudioCoroutineHelper>();
					DontDestroyOnLoad(instance);
				}

				return instance;
			}
		}

		public static Coroutine Run(IEnumerator enumerator) {
			return Instance.StartCoroutine(enumerator);
		}

		public static void Stop(Coroutine routine) {
			Instance.StopCoroutine(routine);
		}
	}
}