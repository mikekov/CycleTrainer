using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CycleApp
{
	static class Utilities
	{
		// partitioning algorithm based upon two adjacent single elements form input collection:
		// if 'partitioner' returns true, new partition starts from the second element
		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> input, Func<T, T, bool> partitioner)
		{
			var seq = new SubEnumerator<T>(input.GetEnumerator());

			while (!seq.Done)
				yield return PartitionHelper(seq, seq.Current, partitioner);
		}

		static IEnumerable<T> PartitionHelper<T>(SubEnumerator<T> input, T element, Func<T, T, bool> partitioner)
		{
			yield return element;

			while (input.Next())
			{
				var next = input.Current;
				if (partitioner(element, next))
					yield break;

				yield return next;

				element = next;
			}
		}

		//---------------------------------------------------------------------------------------------------------

		// partitioning algorithm based upon single element form input collection:
		// as long as 'partitioner' returns the same key, elements are in the same partition;
		// when key changes, new partition starts
		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> input, Func<T, int> partitioner)
		{
			var seq = new SubEnumerator<T>(input.GetEnumerator());

			while (!seq.Done)
			{
				var t = seq.Current;
				var key = partitioner(t);

				yield return PartitionHelper(seq, key, partitioner);
			}
		}

		static IEnumerable<T> PartitionHelper<T>(SubEnumerator<T> input, int key, Func<T, int> partitioner)
		{
			while (true)
			{
				yield return input.Current;

				if (!input.Next())
					yield break;

				var t = input.Current;
				var new_key = partitioner(t);
				if (new_key != key)
					yield break;
			}
		}

		private class SubEnumerator<T>
		{
			public SubEnumerator(IEnumerator<T> e)
			{
				e_ = e;
				spent_ = false;
				Next();
			}

			public bool Next()
			{
				if (e_.MoveNext())
					return true;

				spent_ = true;
				return false;
			}

			public T Current { get { return e_.Current; } }

			public bool Done { get { return spent_; } }

			IEnumerator<T> e_;
			bool spent_;
		}
	}
}
