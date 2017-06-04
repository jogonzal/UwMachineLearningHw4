using System;
using System.Collections.Generic;
using System.Linq;
using Problem4NaiveBayes.DataSet;
using Problem4NaiveBayes.Parsing;
using Problem4NaiveBayes.Statistics;

namespace Problem4NaiveBayes.NaiveBayes
{
	public class NaiveBayesCalculator : IPredictor
	{
		private Dictionary<string, BucketCount> naiveBayesTrainingDataStructure;
		private double probabilityOfOne;

		public NaiveBayesCalculator(double probabilityOfOne, Dictionary<string, BucketCount> naiveBayesTrainingDataStructure)
		{
			this.probabilityOfOne = probabilityOfOne;
			this.naiveBayesTrainingDataStructure = naiveBayesTrainingDataStructure;
		}

		public static Tuple<double, double> ObtainProbabilityOfZeroAndOne(List<int> testValues, Dictionary<string, BucketCount> trainingEmails, double totalProbabilityOne)
		{
			double probabilityZero = 0, probabilityOne = 0;
			double totalProbabilityZero = 1 - totalProbabilityOne;
			for (int index = 0; index < testValues.Count; index++)
			{
				var testValue = testValues[index];
				string key = NaiveBayesDataTransform.BuildKey(index, testValue);
				// Count all the times this word was spam
				BucketCount bucketCount;
				if (!trainingEmails.TryGetValue(key, out bucketCount))
				{
					// laplace smoothing
					bucketCount = new BucketCount();
				}

				const double smoothingNum = 0.1; // Feel free to change this #

				double probabilityOfExampleBeingZero = (1.0*bucketCount.ZeroCount + smoothingNum)/
									(bucketCount.ZeroCount + bucketCount.OneCount + smoothingNum);
				double probabilityOfExampleBeingOne = (1.0*bucketCount.OneCount + smoothingNum)/
									(bucketCount.ZeroCount + bucketCount.OneCount + smoothingNum);

				probabilityZero += Math.Log(probabilityOfExampleBeingZero);
				probabilityOne += Math.Log(probabilityOfExampleBeingOne);
			}

			probabilityZero += Math.Log(totalProbabilityZero);
			probabilityOne += Math.Log(totalProbabilityOne);

			return new Tuple<double, double>(probabilityZero, probabilityOne);
		}

		public bool CalculatePrediction(DataSetValue testExample)
		{
			var probabilityOfZeroAndOne = NaiveBayesCalculator.ObtainProbabilityOfZeroAndOne(testExample.Values,
					naiveBayesTrainingDataStructure, probabilityOfOne);
			bool isOnePrediction = (probabilityOfZeroAndOne.Item2) > probabilityOfZeroAndOne.Item1 * 4.5;
			return isOnePrediction;
		}
	}
}
