using System.Collections.Generic;

namespace Problem1BiasAndVariance.DataSet
{
	public class DataSetAttribute
	{
		public string Name { get; set; }
		public HashSet<string> PossibleValues { get; set; }

		public int ValueIndex { get; private set; }

		public DataSetAttribute(string name, HashSet<string> possibleValues, int valueIndex)
		{
			Name = name;
			PossibleValues = possibleValues;
			ValueIndex = valueIndex;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1} values", Name, PossibleValues.Count);
		}
	}
}
