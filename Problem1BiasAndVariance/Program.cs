using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Problem1BiasAndVariance.DataSet;
using Problem1BiasAndVariance.DecisionTreeClasses;
using Problem1BiasAndVariance.Statistics;

namespace Problem1BiasAndVariance
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
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath);
			Console.WriteLine("Validating training set");
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			Console.WriteLine("TotalSamplesForBiasAndVariance : {0}", TotalSamplesForBiasAndVariance);
			List<int> sizeOfBaggers = new List<int>() { 1, 2, 5, 10 };
			foreach (var sizeOfBagger in sizeOfBaggers)
			{
				Console.WriteLine("Running with SizeOfBaggers {0}", sizeOfBagger);
				// Run the algorithm with different tree depths
				for (int treeDepth = 1; treeDepth <= 3; treeDepth++)
				{
					Console.WriteLine("Running with tree depth {0}", treeDepth);
					RunWithTreeLevels(trainingData, rnd, treeDepth, sizeOfBagger);
				}
			}

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}

		private static void RunWithTreeLevels(ParserResults trainingData, Random rnd, int treeDepth, int sizeOfBaggers)
		{
			List<List<List<DataSetValue>>> dataSetValuesForBagging = new List<List<List<DataSetValue>>>();
			for (int i = 0; i < TotalSamplesForBiasAndVariance; i++)
			{
				dataSetValuesForBagging.Add(Bagging.ProduceDifferentDataSets(trainingData.Values, sizeOfBaggers, rnd));
			}

			// Initialize the required trees
			List<List<DecisionTreeLevel>> listOfTreesToRunTestOn = new List<List<DecisionTreeLevel>>();
			foreach (var dataSetForBagging in dataSetValuesForBagging)
			{
				// Foe each bagger, for each dataset, create a new tree
				listOfTreesToRunTestOn.Add(
					dataSetForBagging.Select(
						dataSet => new DecisionTreeLevel(0, trainingData.Attributes, dataSet, maximumDepth: treeDepth)).ToList());
			}

			Parallel.ForEach(listOfTreesToRunTestOn.SelectMany(s => s), l => l.D3());
			Parallel.ForEach(listOfTreesToRunTestOn.SelectMany(s => s), l => l.TrimTree());

			//string sampleSerializedTree = listOfTreesToRunTestOn[0][0].SerializeDecisionTree();

			//Console.WriteLine("Getting test set...");
			//ParserResults testData = ParserUtils.ParseData(TestSetPath, trainingData.Attributes);
			//Console.WriteLine("Validating test set");
			//DataSetCleaner.ValidateDataSet(testData.Attributes, testData.Values);

			//Console.WriteLine("Evaluating trees against test data...");
			//foreach (List<DecisionTreeLevel> baggingSetOfTrees in listOfTreesToRunTestOn)
			//{
			//	DecisionTreeScore score = DecisionTreeScorer.ScoreWithTreeWithTestSet(baggingSetOfTrees, testData.Values);
			//	score.PrintTotalScore();
			//}

			double variance = 0;
			double bias = 0;
			// Calculate biar and variance for all trees
			foreach (var trainingDataValue in trainingData.Values)
			{
				double realValue = Transformer.BoolToDouble(trainingDataValue.Output);
				List<double> allPredictions = new List<double>(listOfTreesToRunTestOn.Count);
				foreach (var bagger in listOfTreesToRunTestOn)
				{
					bool prediction = DecisionTreeScorer.CalculatePrediction(bagger, trainingDataValue);
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

			variance = variance/trainingData.Values.Count;
			bias = bias/trainingData.Values.Count;

			Console.WriteLine("Variance: {0}. \t Bias: {1}", variance, bias);
			//Console.WriteLine(bias);
			//Console.WriteLine(variance);
		}
	}
}
