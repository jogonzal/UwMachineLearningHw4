using System.Collections.Generic;
using System.Linq;

using Problem4NaiveBayes.DataSet;
using Problem4NaiveBayes.Statistics;

namespace Problem1BiasAndVariance.Statistics
{
	public static class BiasAndVarianceCalculator
	{
		public static double CalculateBiasAndVariance(ParserResults trainingData, List<IPredictor> predictors, out double bias)
		{
			double variance = 0;
			bias = 0;
			// Calculate biar and variance for all trees
			foreach (var trainingDataValue in trainingData.Values)
			{
				double realValue = Transformer.BoolToDouble(trainingDataValue.Output);
				List<double> allPredictions = new List<double>(predictors.Count);
				foreach (var predictor in predictors)
				{
					bool prediction = predictor.CalculatePrediction(trainingDataValue);
					allPredictions.Add(Transformer.BoolToDouble(prediction));
				}
				double mode = ModeFinder.FindMode(allPredictions);
				double averagePrediction = allPredictions.Average();

				// Now that we have the mode, realValue, and average of all predictions, we can calculate variance and bias
				double varianceForThisDataPoint = 0;
				double diffOfRealValueAndAveragePrediction = (realValue - averagePrediction);
				double biasForThisDataPoint = diffOfRealValueAndAveragePrediction * diffOfRealValueAndAveragePrediction;
				foreach (var prediction in allPredictions)
				{
					var diffForModeAndPrediction = prediction - mode;
					varianceForThisDataPoint += diffForModeAndPrediction * diffForModeAndPrediction;
				}
				varianceForThisDataPoint = varianceForThisDataPoint / allPredictions.Count;

				// Accumulate
				variance += varianceForThisDataPoint;
				bias += biasForThisDataPoint;
			}

			variance = variance / trainingData.Values.Count;
			bias = bias / trainingData.Values.Count;
			return variance;
		}
	}
}
