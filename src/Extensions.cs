using System;
using System.Collections.Generic;
using System.Linq;

namespace Cake
{
	public static class Extensions
	{
		 public static void Each<T>(this IEnumerable<T> items, Action<T> action)
		 {
			 foreach (var item in items)
			 {
				 action(item);
			 }
		 }

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems  )
		{
			newItems.Each(collection.Add);
		}

		public static IEnumerable<T> Slice<T>(this IEnumerable<T> items, int start, int end)
		{
			var itemsArray = items.ToArray();

			if (end < 0)
				end = itemsArray.Length + end - 1;

			if (start < 0)
				start = itemsArray.Length + start - 1;

			int increment = (start > end) ? -1 : 1;

			if (start > end)
				throw new IndexOutOfRangeException("start must be less than or equal to end");

			for (int i = start; i <= end; i = i + increment)
				yield return itemsArray[i];
		}
	}
}