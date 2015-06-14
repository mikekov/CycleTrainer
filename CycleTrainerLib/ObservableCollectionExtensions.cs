using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleTrainer
{
	public static class ObservableCollectionExtensions
	{
		//public static void Remove<T>(this ObservableCollection<T> collection)
		//{
		//	var index= files_.FindIndex((pwx) => pwx.FilePath == e.FullPath);
		//	if (index >= 0)
		//		files_.RemoveAt(index);
		//}

	}

	public static class IListExtensions
	{
		public static int BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> key_selector, TKey key)
			where TKey : IComparable<TKey>
		{
			if (list == null)
				throw new ArgumentException("Null 'list'");
			if (key_selector == null)
				throw new ArgumentException("Null 'key_selector'");

			int min= 0;
			int max= list.Count - 1;

			while (min <= max)
			{
				int middle= (max + min) / 2;
				int comp= key_selector(list[middle]).CompareTo(key);

				if (comp < 0)
					min = middle + 1;
				else if (comp > 0)
					max = middle - 1;
				else
					return middle;
			}

			return ~min;
		}

		public enum InsertFlags { AllowDuplicates, NoDuplicates };

		public static int BinarySearchInsert<T, TKey>(this IList<T> list, Func<T, TKey> key_selector, T item, InsertFlags flags= InsertFlags.AllowDuplicates)
			where TKey : IComparable<TKey>
		{
			if (key_selector == null)
				throw new ArgumentException("Null 'key_selector'");

			var index= list.BinarySearch(key_selector, key_selector(item));

			if (index < 0)
				index = ~index;
			else if (flags == InsertFlags.NoDuplicates)
				return ~index;

			list.Insert(index, item);

			return index;
		}


		public static int BinarySearchRemove<T, TKey>(this IList<T> list, Func<T, TKey> key_selector, TKey key)
			where TKey : IComparable<TKey>
		{
			if (key_selector == null)
				throw new ArgumentException("Null 'key_selector'");

			var index= list.BinarySearch(key_selector, key);

			if (index < 0)
				return -1;

			list.RemoveAt(index);

			return index;
		}

	
		public static int IndexOf<T, TKey>(this IList<T> list, Func<T, TKey> key_selector, TKey key)
			where TKey : IEquatable<TKey>
		{
			if (list == null)
				throw new ArgumentException("Null 'list'");
			if (key_selector == null)
				throw new ArgumentException("Null 'key_selector'");

			int index= 0;
			foreach (var item in list)
			{
				if (key_selector(item).Equals(key))
					return index;
				++index;
			}

			return -1;
		}

		public static int Remove<T, TKey>(this IList<T> list, Func<T, TKey> key_selector, TKey key)
			where TKey : IEquatable<TKey>
		{
			var index= list.IndexOf(key_selector, key);

			if (index >= 0)
				list.RemoveAt(index);

			return index;
		}

		public static bool Contains<T, TKey>(this IList<T> list, Func<T, TKey> key_selector, TKey key)
			where TKey : IEquatable<TKey>
		{
			var index= list.IndexOf(key_selector, key);
			return index >= 0;
		}
	}
}
