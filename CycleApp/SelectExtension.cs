using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleApp
{
	public static class SelectExtension
	{
		public static IEnumerable<E> Select2<T, E>(this IEnumerable<T> seq, Func<T, T, E> selector)
		{
			var e = seq.GetEnumerator();

			if (!e.MoveNext())
				yield break;

			T prev = e.Current;

			while (e.MoveNext())
			{
				yield return selector(prev, e.Current);
				prev = e.Current;
			}
		}


		public static E Reduce<T, E>(this IEnumerable<T> seq, Func<T, E, E> operation, E acc = default(E))
		{
			foreach (var el in seq)
				acc = operation(el, acc);

			return acc;
		}
	}
}
