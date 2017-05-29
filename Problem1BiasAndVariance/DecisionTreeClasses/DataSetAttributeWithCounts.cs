using System;
using System.Collections.Generic;
using System.Linq;
using Problem1BiasAndVariance.DataSet;

namespace Problem1BiasAndVariance.DecisionTreeClasses
{
	public class PossibleValuesCounts
	{
		public int AppearCount { get; set; }
		public int AppearWhenTrueCount { get; set; }

		public int AppearWhenFalseCount => AppearCount - AppearWhenTrueCount;

		public PossibleValuesCounts(int appearCount, int appearWhenTrueCount)
		{
			AppearCount = appearCount;
			AppearWhenTrueCount = appearWhenTrueCount;
		}

		public override string ToString()
		{
			return $"{AppearWhenTrueCount} / {AppearCount}";
		}
	}

	public class DataSetAttributeWithCounts : DataSetAttribute
	{
		public Dictionary<string, PossibleValuesCounts> PossibleValueCounts { get; }

		public double Entropy { get; set; }

		public DataSetAttributeWithCounts(string name, HashSet<string> possibleValues, int valueIndex) : base(name, possibleValues, valueIndex)
		{
			PossibleValueCounts = new Dictionary<string, PossibleValuesCounts>(PossibleValues.Count);
			Entropy = -999999999999;
		}

		public void UpdateWith(string attributeValue, bool valueOutput)
		{
			PossibleValuesCounts possibleCount;
			// Add it if it isn't there
			if (!PossibleValueCounts.TryGetValue(attributeValue, out possibleCount))
			{
				possibleCount = new PossibleValuesCounts(0, 0);
				PossibleValueCounts[attributeValue] = possibleCount;
			}

			possibleCount.AppearCount++;
			if (valueOutput)
			{
				possibleCount.AppearWhenTrueCount++;
			}
		}

		public void CalculateEntropy()
		{
			double accumulatedEntropy = 0;
			int totalAppearancesForAllPossibleValues = PossibleValueCounts.Select(s => s.Value.AppearCount).Sum();
			foreach (var possibleValuesCountse in PossibleValueCounts)
			{
				int appearCount = possibleValuesCountse.Value.AppearCount;
				int appearCountWhenTrue = possibleValuesCountse.Value.AppearWhenTrueCount;

				if (appearCountWhenTrue == 0)
				{
					continue;
				}

				double weight = 1.0 * appearCount / totalAppearancesForAllPossibleValues;
				var i = -1.0 * appearCountWhenTrue / appearCount * Math.Log(1.0 * appearCountWhenTrue / appearCount, 2);

				double localEntropy = weight*i;

				accumulatedEntropy += localEntropy;
			}

			Entropy = accumulatedEntropy;
		}

		public override string ToString()
		{
			return string.Format($"{Name}, {Entropy}");
		}
	}
}
