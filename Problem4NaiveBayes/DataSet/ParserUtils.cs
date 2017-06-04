using System.Collections.Generic;
using System.IO;
using System.Linq;
using Problem4NaiveBayes.Statistics;
using Problem4SVM;

namespace Problem4NaiveBayes.DataSet
{
	public class ParserResults
	{
		public List<DataSetAttribute> Attributes { get; set; }
		public List<DataSetValue> Values { get; set; }
		public int Count
		{
			get;
			internal set;
		}

		public ParserResults(List<DataSetAttribute> attributes, List<DataSetValue> values)
		{
			Attributes = attributes;
			Values = values;
		}
	}

	public static class ParserUtils
	{
		public static ParserResults ParseData(string dataSetPath, List<DataSetAttribute> previousAttributes = null, bool convertContinuousValues = true)
		{
			var lines = File.ReadAllLines(dataSetPath);
			List<DataSetValue> rows = new List<DataSetValue>();

			foreach (var line in lines)
			{
				var split = line.Split(new [] {','});

				List<int> values = new List<int>();
				bool isFirst = true;
				foreach (var s in split)
				{
					if (isFirst)
					{
						isFirst = false;
						continue;
					}
					values.Add(int.Parse(s));
				}

				// Remove the last value as it is the class
				int classValue = values[values.Count - 1];
				values.RemoveAt(values.Count - 1);

				rows.Add(new DataSetValue(values, classValue == 1));
			}

			List<DataSetAttribute> attributeList = previousAttributes;
			if (previousAttributes == null)
			{
				int i = 1;
				attributeList = new List<DataSetAttribute>()
				{
					new DataSetAttribute("LIMIT_BAL", null, 1),
					new DataSetAttribute("SEX", new HashSet<int>() {1, 2}, 2),
					new DataSetAttribute("EDUCATION", new HashSet<int>() {0, 1, 2, 3, 4, 5, 6}, 3),
					new DataSetAttribute("MARRIAGE", new HashSet<int>() {0, 1, 2, 3}, 4),
					new DataSetAttribute("AGE", null, 5),
					new DataSetAttribute("PAY_0", new HashSet<int>() {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8}, 6),
					new DataSetAttribute("PAY_2", new HashSet<int>() {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8}, 7),
					new DataSetAttribute("PAY_3", new HashSet<int>() {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8}, 8),
					new DataSetAttribute("PAY_4", new HashSet<int>() {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8}, 9),
					new DataSetAttribute("PAY_5", new HashSet<int>() {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8}, 10),
					new DataSetAttribute("PAY_6", new HashSet<int>() {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8}, 11),
					new DataSetAttribute("BILL_AMT1", null, 12),
					new DataSetAttribute("BILL_AMT2", null, 13),
					new DataSetAttribute("BILL_AMT3", null, 14),
					new DataSetAttribute("BILL_AMT4", null, 15),
					new DataSetAttribute("BILL_AMT5", null, 16),
					new DataSetAttribute("BILL_AMT6", null, 17),
					new DataSetAttribute("PAY_AMT1", null, 18),
					new DataSetAttribute("PAY_AMT2", null, 19),
					new DataSetAttribute("PAY_AMT3", null, 20),
					new DataSetAttribute("PAY_AMT4", null, 21),
					new DataSetAttribute("PAY_AMT5", null, 22),
					new DataSetAttribute("PAY_AMT6", null, 23),
				};

				// Calculate percentiles for all numeric fields and classify them
				foreach (var dataSetAttribute in attributeList)
				{
					if (dataSetAttribute.PossibleValues == null)
					{
						dataSetAttribute.IsContinuous = true;
						dataSetAttribute.Percentiles = PercentileCalculation.CalculatePercentiles(rows, dataSetAttribute.ValueIndex);
					}
				}
			}

			// Turn percentile values into buckets
			foreach (var dataSetValue in rows)
			{
				foreach (var dataSetAttribute in attributeList)
				{
					if (dataSetAttribute.IsContinuous && convertContinuousValues)
					{
						int currentValue = dataSetValue.Values[dataSetAttribute.ValueIndex];
						var percentiles = dataSetAttribute.Percentiles;
						int tranformedValue = PercentileCalculation.CalculatePercentileBucket(dataSetAttribute, currentValue);
						dataSetValue.Values[dataSetAttribute.ValueIndex] = tranformedValue;
						dataSetAttribute.PossibleValues = new HashSet<int>(Enumerable.Range(0, Program.PercentilesToBreakIn + 1));
					}
				}
			}

			return new ParserResults(attributeList, rows);
		}
	}
}
