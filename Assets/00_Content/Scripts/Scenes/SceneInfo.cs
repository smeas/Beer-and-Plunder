using System;
using Audio;
using UnityEngine;

namespace Scenes {
	[Serializable]
	public class SceneInfo {
		[SerializeField] public SceneReference scene;
		[SerializeField] public MusicCue music;

		public void Load() {
			scene.Load();
			// Tell music player to switch music / Play scene transition m.m
		}

		public void LoadAsync() {
			scene.LoadAsync();
			// Tell music player to switch music / Play scene transition m.m
		}
	}
}
