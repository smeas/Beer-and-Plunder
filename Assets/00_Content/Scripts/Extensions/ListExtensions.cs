using System.Collections.Generic;

namespace Extensions {
	public static class ListExtensions {
		public static void Swap<T>(this IList<T> list, int index1, int index2) {
			T temp = list[index1];
			list[index1] = list[index2];
			list[index2] = temp;
		}

		/// <summary>
		/// Remove the last item from the list and return it.
		/// </summary>
		public static T Pop<T>(this IList<T> list) {
			T item = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return item;
		}

		/// <summary>
		/// Remove an item from the list leaving the last item in its place.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="index">The index of the item to remove.</param>
		public static void SwapRemoveAt<T>(this IList<T> list, int index) {
			if (index == list.Count - 1) {
				list.RemoveAt(index);
				return;
			}

			list.Swap(index, list.Count - 1);
			list.RemoveAt(list.Count - 1);
		}

		/// <summary>
		/// Remove an item from the list leaving the last item in its place.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="item">The item to remove.</param>
		public static bool SwapRemove<T>(this IList<T> list, T item) {
			int index = list.IndexOf(item);
			if (index != -1) {
				SwapRemoveAt(list, index);
				return true;
			}

			return false;
		}
	}
}