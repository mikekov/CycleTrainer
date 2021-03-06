﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleTrainer
{
	public static class EnumerableExtensions
	{
		///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
		///<param name="items">The enumerable to search.</param>
		///<param name="predicate">The expression to test the items against.</param>
		///<returns>The index of the first matching item, or -1 if no items match.</returns>
		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			int index= 0;
			foreach (var item in items)
			{
				if (predicate(item))
					return index;

				++index;
			}

			return -1;
		}
	}
}
