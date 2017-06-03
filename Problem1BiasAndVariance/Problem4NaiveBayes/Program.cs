using System;
using System.Collections.Generic;
using System.IO;
using Problem4NaiveBayes.DataSet;
using Problem4NaiveBayes.NaiveBayes;
using Problem4NaiveBayes.Parsing;

namespace Problem4SVM
{
	class Program
	{
		public static int PercentilesToBreakIn = 10;
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

			Random rnd = new Random();
			Console.WriteLine("Reading training data...");
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath, convertContinuousValues: true);
			Console.WriteLine("Validating training set");
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			Console.WriteLine("Getting test set...");
			ParserResults testData = ParserUtils.ParseData(TestSetPath, trainingData.Attributes, convertContinuousValues: true);
			Console.WriteLine("Validating test set");
			DataSetCleaner.ValidateDataSet(testData.Attributes, testData.Values);

			Dictionary<string, BucketCount> naiveBayesTrainingDataStructure = NaiveBayesDataTransform.CountSamples(trainingData.Values);



			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}
	}
}
