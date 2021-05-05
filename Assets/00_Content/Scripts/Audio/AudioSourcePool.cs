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
		private readonly LinkedList<(AudioSource source, SoundHandle handle)> usedSources = new LinkedList<(AudioSource, SoundHandle)>();

		private static readonly SoundHandle nullSoundHandle = new SoundHandle();

		public int Size => freeSources.Count + usedSources.Count;
		public int MaxSize => maxSources;

		public AudioSourcePool(AudioSource prefab, int maxSources, Transform container, string poolName) {
			this.maxSources = maxSources;
			this.prefab = prefab;
			this.container = container;
			this.poolName = poolName;
		}

		public SoundHandle PlayOneShot(AudioClip clip, bool loop = false, float volume = 1f, float pitch = 1f) {
			(AudioSource source, SoundHandle handle) = GetSource();
			if (source == null)
				return nullSoundHandle;

			source.clip = clip;
			source.volume = volume;
			source.pitch = pitch;
			source.loop = loop;
			source.Play();
			return handle;
		}

		private (AudioSource, SoundHandle) GetSource() {
			int currentFrame = Time.frameCount;
			if (currentFrame > lastCleanupFrame) {
				CleanUpSources();
				lastCleanupFrame = currentFrame;
			}

			if (freeSources.Count > 0) {
				AudioSource source = freeSources.Dequeue();
				SoundHandle handle = new SoundHandle(source);
				usedSources.AddLast((source, handle));
				return (source, handle);
			}
			else if (Size < maxSources) {
				AudioSource source = CreateSource();
				SoundHandle handle = new SoundHandle(source);
				usedSources.AddLast((source, handle));
				return (source, handle);
			}
			else {
				Debug.LogWarning("No more sources");
				return (null, nullSoundHandle);
			}
		}

		private void CleanUpSources() {
			LinkedListNode<(AudioSource source, SoundHandle handle)> node = usedSources.First;

			while (node != null) {
				LinkedListNode<(AudioSource, SoundHandle)> nextNode = node.Next;

				if (!node.Value.source.isPlaying) {
					node.Value.handle.Invalidate();
					freeSources.Enqueue(node.Value.source);
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