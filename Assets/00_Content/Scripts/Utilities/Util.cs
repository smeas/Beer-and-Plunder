using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Utilities {
	public static class Util {
		public static T RandomElement<T>(IList<T> list) {
			return list[Random.Range(0, list.Count)];
		}

		// https://en.wikipedia.org/wiki/Reservoir_sampling
		public static T[] RandomSample<T>(IList<T> list, int sampleCount) {
			if (list.Count <= sampleCount)
				throw new ArgumentOutOfRangeException(nameof(sampleCount), "List does not contain enough items.");

			T[] result = list.Take(sampleCount).ToArray();
			for (int i = sampleCount; i < list.Count; i++) {
				int j = Random.Range(0, i);
				if (j < sampleCount)
					result[j] = list[i];
			}

			return result;
		}
	}
}