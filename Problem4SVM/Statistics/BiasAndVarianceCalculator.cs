using System.Collections.Generic;
using System.Linq;

using Problem4SVM.DataSet;

namespace Problem1BiasAndVariance.Statistics
{
	public static class BiasAndVarianceCalculator
	{
		public static double CalculateBiasAndVariance(ParserResults trainingData, List<List<bool>> predictorsPredictions, out double bias)
		{
			double variance = 0;
			bias = 0;
			// Calculate biar and variance for all trees
			for (int trainingDataIndex = 0; trainingDataIndex < trainingData.Values.Count; trainingDataIndex++)
			{
				var trainingDataValue = trainingData.Values[trainingDataIndex];
				double realValue = Transformer.BoolToDouble(trainingDataValue.Output);
				List<double> allPredictions = new List<double>(predictorsPredictions.Count);
				foreach (var predictorPredictions in predictorsPredictions)
				{
					bool prediction = predictorPredictions[trainingDataIndex];
					allPredictions.Add(Transformer.BoolToDouble(prediction));
				}
				double mode = ModeFinder.FindMode(allPredictions);
				double averagePrediction = allPredictions.Average();

				// Now that we have the mode, realValue, and average of all predictions, we can calculate variance and bias
				double varianceForThisDataPoint = 0;
				double diffOfRealValueAndAveragePrediction = (realValue - averagePrediction);
				double biasForThisDataPoint = diffOfRealValueAndAveragePrediction*diffOfRealValueAndAveragePrediction;
				foreach (var prediction in allPredictions)
				{
					var diffForModeAndPrediction = prediction - mode;
					varianceForThisDataPoint += diffForModeAndPrediction*diffForModeAndPrediction;
				}
				varianceForThisDataPoint = varianceForThisDataPoint/allPredictions.Count;

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
