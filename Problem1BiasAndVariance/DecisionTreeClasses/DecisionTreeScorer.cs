using System;
using System.Collections.Generic;
using System.Linq;
using Problem1BiasAndVariance.DataSet;

namespace Problem1BiasAndVariance.DecisionTreeClasses
{
	public class DecisionTreeScore
	{
		private readonly string _decisionTreeDescription;
		public double PositiveHit { get; set; }
		public double FalsePositive { get; set; }
		public double NegativeHits { get; set; }
		public double FalseNegative { get; set; }
		public int NodeCount { get; set; }

		public DecisionTreeScore(double positiveHit, double falsePositive, double negativeHits, double falseNegative, string decisionTreeDescription)
		{
			_decisionTreeDescription = decisionTreeDescription;
			PositiveHit = positiveHit;
			FalsePositive = falsePositive;
			NegativeHits = negativeHits;
			FalseNegative = falseNegative;
		}

		public double GetTotalScore()
		{
			return (PositiveHit + NegativeHits)/(PositiveHit + NegativeHits + FalsePositive + FalseNegative);
		}

		public void PrintTotalScore()
		{
			Console.WriteLine($"Score: {GetTotalScore()}");
			//Console.WriteLine($"PositiveHits:{1.0*PositiveHit/(FalsePositive + PositiveHit)} NegativeHit:{1.0*NegativeHits/(FalseNegative + NegativeHits)}");
		}
	}

	public static class DecisionTreeScorer
	{
		public static DecisionTreeScore ScoreWithTreeWithTestSet(DecisionTreeLevel decisionTree, List<DataSetValue> testDataSetValues)
		{
			return ScoreWithTreeWithTestSet(new List<DecisionTreeLevel>() {decisionTree}, testDataSetValues);
		}

		public static DecisionTreeScore ScoreWithTreeWithTestSet(List<DecisionTreeLevel> decisionTrees, List<DataSetValue> testDataSetValues)
		{
			DecisionTreeScore score = new DecisionTreeScore(0, 0, 0, 0, "count:" + decisionTrees.Count);
			foreach (var testDataSetValue in testDataSetValues)
			{
				// Poll the trees
				var output = CalculatePrediction(decisionTrees, testDataSetValue);

				if (output && testDataSetValue.Output)
				{
					score.PositiveHit++;
				}
				else if (!output && !testDataSetValue.Output)
				{
					score.NegativeHits++;
				}
				else if (output && !testDataSetValue.Output)
				{
					score.FalsePositive++;
				}
				else if (!output && testDataSetValue.Output)
				{
					score.FalseNegative++;
				}
			}

			score.NodeCount = decisionTrees.Sum(s => s.GetNodeCount());

			return score;
		}

		public static bool CalculatePrediction(List<DecisionTreeLevel> decisionTrees, DataSetValue inputValues)
		{
			int positiveCount = 0, negativeCount = 0;
			foreach (var decisionTree in decisionTrees)
			{
				bool localOutput = decisionTree.Evaluate(inputValues.Values);
				if (localOutput)
				{
					positiveCount++;
				}
				else
				{
					negativeCount++;
				}
			}
			bool output = positiveCount > negativeCount;
			return output;
		}
	}
}
