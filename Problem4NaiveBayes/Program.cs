using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Problem4NaiveBayes;
using Problem4NaiveBayes.DataSet;
using Problem4NaiveBayes.NaiveBayes;
using Problem4NaiveBayes.Parsing;
using Problem4NaiveBayes.Statistics;

namespace Problem4SVM
{
	class Program
	{
		public static int PercentilesToBreakIn = 5;
		public static int TotalSamplesForBiasAndVariance = 30;

		private static string DataSetPath => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\bankddefaultdataset\training.csv");
		private static string TestSetPath => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\bankddefaultdataset\testing.csv");

		static void Main(string[] args)
		{
			string errorMessage = "";
			if (!File.Exists(DataSetPath))
			{
				errorMessage += $"Failed to find file ${DataSetPath} - please update variable ${nameof(DataSetPath)} or create that file.\n";
			}
			if (!File.Exists(TestSetPath))
			{
				errorMessage += $"Failed to find file ${TestSetPath} - please update variable ${nameof(TestSetPath)} or create that file.\n";
			}

			if (errorMessage != "")
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Not all files available - not running!");
				Console.WriteLine(errorMessage);
				Console.ResetColor();
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
				return;
			}

			var startTime = DateTime.Now;
			Console.WriteLine(startTime);

			Random rnd = new Random();
			Console.WriteLine("Reading training data...");
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath, convertContinuousValues: true);
			Console.WriteLine("Validating training set");
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			Console.WriteLine("Getting test set...");
			ParserResults testData = ParserUtils.ParseData(TestSetPath, trainingData.Attributes, convertContinuousValues: true);
			Console.WriteLine("Validating test set");
			DataSetCleaner.ValidateDataSet(testData.Attributes, testData.Values);

			List<List<DataSetValue>> differentDataSets = Bagging.ProduceDifferentDataSets(trainingData.Values, TotalSamplesForBiasAndVariance, rnd);
			List<IPredictor> naiveBayesPredictors = new List<IPredictor>();
			foreach (var differentTrainingDataSet in differentDataSets)
			{
				var naiveBayesPredictor = TrainNaiveBayes(differentTrainingDataSet);
				naiveBayesPredictors.Add(naiveBayesPredictor);
			}

			double bias;
			double variance = BiasAndVarianceCalculator.CalculateBiasAndVariance(trainingData, naiveBayesPredictors, out bias);

			Console.WriteLine($"Bias:{bias} Variance:{variance}");

			var originalNaiveBayesPredictor = TrainNaiveBayes(trainingData.Values);
			EvaluateNaiveBayesPredictor(testData, originalNaiveBayesPredictor);

			var endTime = DateTime.Now;
			Console.WriteLine(endTime);
			var totalMinutes = (endTime - startTime).TotalMinutes;
			Console.WriteLine("Took {0} minutes.", totalMinutes);
			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}

		private static void EvaluateNaiveBayesPredictor(ParserResults testData, NaiveBayesCalculator naiveBayesPredictor)
		{
			Console.WriteLine("Making predictions...");
			uint hits = 0, misses = 0;
			uint falsePositives = 0, falseNegatives = 0;
			foreach (var testExample in testData.Values)
			{
				var isOnePrediction = naiveBayesPredictor.CalculatePrediction(testExample);
				if (isOnePrediction && testExample.Output)
				{
					hits++;
				}
				else if (!isOnePrediction && !testExample.Output)
				{
					hits++;
				}
				else if (isOnePrediction && !testExample.Output)
				{
					misses++;
					falsePositives++;
				}
				else if (!isOnePrediction && testExample.Output)
				{
					misses++;
					falseNegatives++;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			Console.WriteLine("Score: {0}%. Hits: {1}, Misses: {2}", 100.0*hits/(misses + hits), hits, misses);
			Console.WriteLine("FalsePositives: {0}. FalseNegatives: {1}", falsePositives, falseNegatives);
		}

		private static NaiveBayesCalculator TrainNaiveBayes(List<DataSetValue> trainingValues)
		{
			Dictionary<string, BucketCount> naiveBayesTrainingDataStructure =
				NaiveBayesDataTransform.CountSamples(trainingValues);

			double probabilityOfOne = 1.0* trainingValues.Count(t => t.Output)/ trainingValues.Count;

			var bayesPredictor = new NaiveBayesCalculator(probabilityOfOne, naiveBayesTrainingDataStructure);

			return bayesPredictor;
		}
	}
}
