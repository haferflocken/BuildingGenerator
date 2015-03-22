using UnityEngine;
using System.Collections.Generic;
using System.Text;

public static class IEnumerableExtensions
{
	public static void EnqueueAll<T>(this Queue<T> destination, IEnumerable<T> source)
	{
		foreach (T x in source)
		{
			destination.Enqueue(x);
		}
	}

	public static List<int> FindAllIndexes<T>(this IList<T> list, System.Predicate<T> match)
	{
		List<int> indexes = new List<int>();
		for (int i = 0; i < list.Count; ++i)
		{
			T t = list[i];
			if (match(t))
			{
				indexes.Add(i);
			}
		}
		return indexes;
	}

	public static string DeepToString<T>(this IEnumerable<T> enumerable)
	{
		StringBuilder builder = new StringBuilder();
		foreach (T t in enumerable)
		{
			if (t is IEnumerable<T>)
			{
				IEnumerable<T> tEnumerable = t as IEnumerable<T>;
				builder.AppendLine(tEnumerable.DeepToString<T>());
			}
			else
			{
				builder.AppendLine(t.ToString());
			}
		}
		return builder.ToString().Trim();
	}

	public delegate bool EqualityCheck<T>(T left, T right);
	public static void DetectDuplicates<T>(this IList<T> list, EqualityCheck<T> comparer, string warnMessage)
	{
		for (int i = 0; i < list.Count; ++i)
		{
			for (int j = i + 1; j < list.Count; ++j)
			{
				if (comparer(list[i], list[j]))
				{
					Debug.LogWarning(warnMessage);
				}
			}
		}
	}

	public static void DetectDuplicates<T>(this IList<T> list, string warnMessage)
	{
		list.DetectDuplicates((left, right) => left.Equals(right), warnMessage);
	}

	public static T FirstElem<T>(this IEnumerable<T> enumerable)
	{
		foreach (T t in enumerable)
		{
			return t;
		}
		return default(T);
	}
	
	public static T MinBy<T, TResult>(this IEnumerable<T> enumerable, System.Func<T, TResult> operation) where TResult : System.IComparable<TResult>
	{
		T min = enumerable.FirstElem();
		TResult minComp = operation(min);

		foreach (T t in enumerable)
		{
			TResult comp = operation(t);
			if (comp.CompareTo(minComp) < 0)
			{
				min = t;
				minComp = comp;
			}
		}

		return min;
	}
}
