using System.Collections.Generic;

namespace Problem1BiasAndVariance.DataSet
{
	public class DataSetValue
	{
		public List<string> Values { get; set; }
		public bool Output { get; set; }

		public DataSetValue(List<string> values, bool output)
		{
			Values = values;
			Output = output;
		}
	}
}
