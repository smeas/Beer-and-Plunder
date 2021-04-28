﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scenes {
	public class SceneTransition : MonoBehaviour {
		private static readonly int radiusID = Shader.PropertyToID("_Radius");

		[SerializeField] private Image overlay;

		[Header("Settings")]
		[SerializeField] private float transitionDuration;

		private bool hasActiveTransition;
		private AsyncOperation loadOperation;
		private Material overlayMaterial;

		private void Awake() {
			overlayMaterial = new Material(overlay.material);
			overlay.material = overlayMaterial;
		}

		public void Play(SceneInfo nextScene) {
			if (hasActiveTransition) return;

			loadOperation = SceneManager.LoadSceneAsync(nextScene.scene);
			loadOperation.allowSceneActivation = false;

			hasActiveTransition = true;
			gameObject.SetActive(true);

			StartCoroutine(CoExitScene());
		}

		private IEnumerator CoExitScene() {
			float exitDuration = transitionDuration / 2;

			for (float i = exitDuration; i > 0; i -= Time.deltaTime) {
				overlayMaterial.SetFloat(radiusID, i / exitDuration);
				yield return null;
			}

			// Wait until scene is ready to open
			while (loadOperation.progress < 0.9f)
				yield return null;

			StartCoroutine(CoEnterScene());
		}

		private IEnumerator CoEnterScene() {
			loadOperation.allowSceneActivation = true;

			float enterDuration = transitionDuration / 2;

			for (float i = 0; i < enterDuration; i += Time.deltaTime) {
				overlayMaterial.SetFloat(radiusID, i / enterDuration);
				yield return null;
			}

			hasActiveTransition = false;
			gameObject.SetActive(false);
		}
	}
}
