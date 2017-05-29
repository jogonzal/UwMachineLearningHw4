using System;
using System.Collections.Generic;

namespace Problem1BiasAndVariance.DataSet
{
	public static class DataSetCleaner
	{
		public static void ValidateDataSet(List<DataSetAttribute> attributes, List<DataSetValue> dataSetValues)
		{
			foreach (var dataSetValue in dataSetValues)
			{
				foreach (var dataSetAttribute in attributes)
				{
					string val = dataSetValue.Values[dataSetAttribute.ValueIndex];
					bool isValueAllowed = dataSetAttribute.PossibleValues.Contains(val);

					if (val == "?" && !isValueAllowed)
					{
						// questionmarks are nulls
						dataSetAttribute.PossibleValues.Add("NULL");
						dataSetValue.Values[dataSetAttribute.ValueIndex] = "NULL";
						continue;
					}

					if (!isValueAllowed)
					{
						throw new InvalidOperationException($"Failed to find value {val} of type {dataSetAttribute.Name}. Allowed list is {string.Join(",", dataSetAttribute.PossibleValues)}");
					}
				}
			}
		}
	}
}
