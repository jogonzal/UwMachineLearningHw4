using System.Collections.Generic;

namespace Problem1BiasAndVariance.DataSet
{
	public class DataSetValue
	{
		public List<int> Values { get; set; }
		public bool Output { get; set; }

		public DataSetValue(List<int> values, bool output)
		{
			Values = values;
			Output = output;
		}
	}
}
