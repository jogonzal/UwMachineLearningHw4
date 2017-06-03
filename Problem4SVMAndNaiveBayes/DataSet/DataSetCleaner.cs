using System;
using System.Collections.Generic;

namespace Problem4SVMAndNaiveBayes.DataSet
{
	public static class DataSetCleaner
	{
		public static void ValidateDataSet(List<DataSetAttribute> attributes, List<DataSetValue> dataSetValues)
		{
			foreach (var dataSetValue in dataSetValues)
			{
				foreach (var dataSetAttribute in attributes)
				{
					var val = dataSetValue.Values[dataSetAttribute.ValueIndex];
					if (dataSetAttribute.IsContinuous && dataSetAttribute.PossibleValues == null)
					{
						continue;
					}

					bool isValueAllowed = dataSetAttribute.PossibleValues.Contains(val);

					if (!isValueAllowed)
					{
						throw new InvalidOperationException($"Failed to find value {val} of type {dataSetAttribute.Name}. Allowed list is {string.Join(",", dataSetAttribute.PossibleValues)}");
					}
				}
			}
		}
	}
}
