using System.Collections.Generic;

namespace Problem4SVMAndNaiveBayes.DataSet
{
	public class DataSetAttribute
	{
		public string Name { get; set; }
		public HashSet<int> PossibleValues { get; set; }

		public int ValueIndex { get; private set; }
		public List<int> Percentiles { get; set; }
		public bool IsContinuous { get; set; }

		public DataSetAttribute(string name, HashSet<int> possibleValues, int valueIndex)
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
