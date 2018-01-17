using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Template.Extensions
{
	public static class ListExtensions
	{

		public static TSource LastOrNew<TSource>(this IEnumerable<TSource> source) where TSource : new()
		{
			var last = source.LastOrDefault();

			if (last == null)
				last = new TSource();

			return last;
		}

		public static TSource FirstOrNew<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : new()
		{
			return source.Where(predicate).ToList().FirstOrNew();
		}


		public static TSource FirstOrNew<TSource>(this IEnumerable<TSource> source) where TSource : new()
		{
			var first = source.FirstOrDefault();

			if (first == null)
				first = new TSource();

			return first;
		}

		public static List<List<T>> Split<T>(this List<T> items, int sliceSize = 30)
		{
			List<List<T>> list = new List<List<T>>();
			for (int i = 0; i < items.Count; i += sliceSize)
				list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
			return list;
		}


		public static bool Empty<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
				return true;

			return !source.Any();
		}

		public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
		{
			foreach (var item in source)
			{
				yield return item;
				foreach (var child in childrenSelector(item).Flatten(childrenSelector))
				{
					yield return child;
				}
			}
		}
	}
}
