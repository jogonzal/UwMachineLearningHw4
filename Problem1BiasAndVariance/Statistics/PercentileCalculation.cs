using System.Collections.Generic;

using Problem1BiasAndVariance.DataSet;

namespace Problem1BiasAndVariance.Statistics
{
	public static class PercentileCalculation
	{
		private static List<int> CalculatePercentiles(List<int> numbers)
		{
			numbers.Sort();

			List<int> percentiles = new List<int>(Program.PercentilesToBreakIn);

			int numbersPerPercentile = numbers.Count / Program.PercentilesToBreakIn;
			for (int i = 0; i < Program.PercentilesToBreakIn; i++)
			{
				int position = i*numbersPerPercentile;
				int percentile = numbers[position];
				percentiles.Add(percentile);
			}

			return percentiles;
		}

		internal static List<int> CalculatePercentiles(List<DataSetValue> rows, int valueIndex)
		{
			List<int> numbers = new List<int>(rows.Count);
			foreach (var dataSetValue in rows)
			{
				numbers.Add(dataSetValue.Values[valueIndex]);
			}
			return CalculatePercentiles(numbers);
		}

		public static int CalculatePercentileBucket(DataSetAttribute attribute, int value)
		{
			int i;
			for (i = 0; i < attribute.Percentiles.Count; i++)
			{
				int percentileLimit = attribute.Percentiles[i];
				if (value > percentileLimit)
				{
					continue;
				}
				return i;
			}

			return i;
		}
	}
}
