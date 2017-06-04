using System.Collections.Generic;

using Problem4NaiveBayes.DataSet;
using Problem4NaiveBayes.Parsing;

namespace Problem4NaiveBayes.NaiveBayes
{
	public static class NaiveBayesDataTransform
	{
		public static Dictionary<string, BucketCount> CountSamples(List<DataSetValue> rows)
		{
			var bucketCounts = new Dictionary<string, BucketCount>();
			for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
			{
				var row = rows[rowIndex];

				bool output = row.Output;
				for (int index = 0; index < row.Values.Count; index++)
				{
					var rowValue = row.Values[index];
					string bucketKey = BuildKey(index, rowValue);

					BucketCount bucketCount;
					if (!bucketCounts.TryGetValue(bucketKey, out bucketCount))
					{
						bucketCount = new BucketCount();
						bucketCounts.Add(bucketKey, bucketCount);
					}
					bucketCount.Add(output);
				}
			}

			return bucketCounts;
		}

		public static string BuildKey(int index, int value)
		{
			return index + ":" + value;
		}
	}
}
