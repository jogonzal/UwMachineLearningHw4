using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Problem4NaiveBayes.DataSet;
using Problem4NaiveBayes.NaiveBayes;
using Problem4NaiveBayes.Parsing;

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

			Dictionary<string, BucketCount> naiveBayesTrainingDataStructure = NaiveBayesDataTransform.CountSamples(trainingData.Values, trainingData.Attributes);

			double probabilityOfOne = 1.0 * trainingData.Values.Count(t => t.Output) / trainingData.Values.Count;

			Console.WriteLine("Making predictions...");
			uint hits = 0, misses = 0;
			uint falsePositives = 0, falseNegatives = 0;
			foreach (var testExample in testData.Values)
			{
				var probabilityOfZeroAndOne = NaiveBayesCalculator.ObtainProbabilityOfZeroAndOne(testExample.Values, naiveBayesTrainingDataStructure, probabilityOfOne);
				bool isOnePrediction = (probabilityOfZeroAndOne.Item2 + 27/* Offset adjustment */) > probabilityOfZeroAndOne.Item1;
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

			Console.WriteLine("Score: {0}%. Hits: {1}, Misses: {2}", 100.0 * hits / (misses + hits), hits, misses);
			Console.WriteLine("FalsePositives: {0}. FalseNegatives: {1}", falsePositives, falseNegatives);

			var endTime = DateTime.Now;
			Console.WriteLine(endTime);
			var totalMinutes = (endTime - startTime).TotalMinutes;
			Console.WriteLine("Took {0} minutes.", totalMinutes);
			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}
	}
}
