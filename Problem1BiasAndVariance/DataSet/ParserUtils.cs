using System.Collections.Generic;
using System.IO;
using System.Linq;
using Problem1BiasAndVariance.Statistics;

namespace Problem1BiasAndVariance.DataSet
{
	public class ParserResults
	{
		public List<DataSetAttribute> Attributes { get; set; }
		public List<DataSetValue> Values { get; set; }

		public ParserResults(List<DataSetAttribute> attributes, List<DataSetValue> values)
		{
			Attributes = attributes;
			Values = values;
		}
	}

	public static class ParserUtils
	{
		public static ParserResults ParseData(string dataSetPath)
		{
			var lines = File.ReadAllLines(dataSetPath);
			List<DataSetValue> rows = new List<DataSetValue>();

			foreach (var line in lines)
			{
				var split = line.Split(new [] {','});

				List<int> values = new List<int>();
				foreach (var s in split)
				{
					values.Add(int.Parse(s));
				}

				// Remove the last value as it is the class
				int classValue = values[values.Count - 1];
				values.RemoveAt(values.Count - 1);

				rows.Add(new DataSetValue(values, classValue == 1));
			}

			int i = 1;
			var attributeList = new List<DataSetAttribute>()
			{
				new DataSetAttribute("LIMIT_BAL", null, i++),
				new DataSetAttribute("SEX", new HashSet<int>() {1, 2}, i++),
				new DataSetAttribute("EDUCATION", new HashSet<int>() {1, 2, 3}, i++),
				new DataSetAttribute("MARRIAGE", new HashSet<int>() {1, 2}, i++),
				new DataSetAttribute("AGE", null, i++),
				new DataSetAttribute("PAY_0", null, i++),
				new DataSetAttribute("PAY_2", null, i++),
				new DataSetAttribute("PAY_3", null, i++),
				new DataSetAttribute("PAY_4", null, i++),
				new DataSetAttribute("PAY_5", null, i++),
				new DataSetAttribute("PAY_6", null, i++),
				new DataSetAttribute("BILL_AMT1", null, i++),
				new DataSetAttribute("BILL_AMT2", null, i++),
				new DataSetAttribute("BILL_AMT3", null, i++),
				new DataSetAttribute("BILL_AMT4", null, i++),
				new DataSetAttribute("BILL_AMT5", null, i++),
				new DataSetAttribute("BILL_AMT6", null, i++),
				new DataSetAttribute("PAY_AMT1", null, i++),
				new DataSetAttribute("PAY_AMT2", null, i++),
				new DataSetAttribute("PAY_AMT3", null, i++),
				new DataSetAttribute("PAY_AMT4", null, i++),
				new DataSetAttribute("PAY_AMT5", null, i++),
				new DataSetAttribute("PAY_AMT6", null, i++),
			};

			// Calculate percentiles for all numeric fields and classify them
			foreach (var dataSetAttribute in attributeList)
			{
				if (dataSetAttribute.PossibleValues == null)
				{
					dataSetAttribute.IsContinuous = true;
					dataSetAttribute.Percentiles = PercentileCalculation.CalculatePercentiles(rows, dataSetAttribute.ValueIndex);
					dataSetAttribute.PossibleValues = new HashSet<int>(Enumerable.Range(0, Program.PercentilesToBreakIn));
				}
			}

			// Turn percentile values into buckets
			foreach (var dataSetValue in rows)
			{
				foreach (var dataSetAttribute in attributeList)
				{
					if (dataSetAttribute.IsContinuous)
					{
						dataSetValue.Values[dataSetAttribute.ValueIndex] =
							PercentileCalculation.CalculatePercentileBucket(dataSetAttribute,
								dataSetValue.Values[dataSetAttribute.ValueIndex]);
					}
				}
			}

			return new ParserResults(attributeList, rows);
		}
	}
}
