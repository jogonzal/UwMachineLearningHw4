using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Problem1BiasAndVariance.DataSet
{
	public static class DataParser
	{
		public static List<DataSetValue> ReadInCSV(string path)
		{
			List<DataSetValue> dataSetValues = new List<DataSetValue>();
			var lines = File.ReadAllLines(path);
			foreach (var line in lines)
			{
				if (!line.StartsWith("+") && !line.StartsWith("-"))
				{
					continue;
				}

				string[] values = line.Split(new[] {','});
				string output = values[0];
				List<string> rest = values.Skip(0).ToList();

				dataSetValues.Add(new DataSetValue(rest, output == "+"));
			}

			return dataSetValues;
		}
	}
}
