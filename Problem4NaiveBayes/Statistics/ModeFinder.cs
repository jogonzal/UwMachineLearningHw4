using System;
using System.Collections.Generic;
using System.Linq;

namespace Problem4NaiveBayes.Statistics
{
	public static class ModeFinder
	{
		public static T FindMode<T>(List<T> list) where T:IComparable
		{
			var mode = list.GroupBy(i => i)
			.OrderByDescending(g => g.Count())
			.Select(g => g.Key)
			.First();


			return mode;
		}
	}
}
