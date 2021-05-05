using System.Collections.Generic;
using UnityEngine;

namespace Audio {
	public class AudioSourcePool {
		private readonly int maxSources;
		private readonly AudioSource prefab;
		private readonly Transform container;
		private readonly string poolName;
		private int lastCleanupFrame = -1;

		private readonly Queue<AudioSource> freeSources = new Queue<AudioSource>();
		private readonly LinkedList<AudioSource> usedSources = new LinkedList<AudioSource>();

		public int Size => freeSources.Count + usedSources.Count;
		public int MaxSize => maxSources;

		public AudioSourcePool(AudioSource prefab, int maxSources, Transform container, string poolName) {
			this.maxSources = maxSources;
			this.prefab = prefab;
			this.container = container;
			this.poolName = poolName;
		}

		public void PlayOneShot(AudioClip clip) => PlayOneShot(clip, Vector3.zero);

		public void PlayOneShot(AudioClip clip, Vector3 position) {
			AudioSource source = GetSource();
			if (source == null)
				return;

			source.clip = clip;
			source.transform.position = position;
			source.Play();
		}

		private AudioSource GetSource() {
			int currentFrame = Time.frameCount;
			if (currentFrame > lastCleanupFrame) {
				CleanUpSources();
				lastCleanupFrame = currentFrame;
			}

			if (freeSources.Count > 0) {
				AudioSource source = freeSources.Dequeue();
				usedSources.AddLast(source);
				return source;
			}
			else if (Size < maxSources) {
				AudioSource source = CreateSource();
				usedSources.AddLast(source);
				return source;
			}
			else {
				Debug.LogWarning("No more sources");
				return null;
			}
		}

		private void CleanUpSources() {
			LinkedListNode<AudioSource> node = usedSources.First;

			while (node != null) {
				LinkedListNode<AudioSource> nextNode = node.Next;

				if (!node.Value.isPlaying) {
					freeSources.Enqueue(node.Value);
					usedSources.Remove(node);
				}

				node = nextNode;
			}
		}

		private AudioSource CreateSource() {
			AudioSource audioSource = Object.Instantiate(prefab, container);
		#if UNITY_EDITOR
			audioSource.gameObject.name = $"{poolName} {Size}";
		#endif
			return audioSource;
		}
	}
}