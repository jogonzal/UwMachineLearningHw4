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

			Console.WriteLine("Getting test set...");
			ParserResults testData = ParserUtils.ParseData(TestSetPath, trainingData.Attributes);
			Console.WriteLine("Validating test set");
			DataSetCleaner.ValidateDataSet(testData.Attributes, testData.Values);

			Console.WriteLine("TotalSamplesForBiasAndVariance : {0}", TotalSamplesForBiasAndVariance);
			List<int> sizeOfBaggers = new List<int>() { 1, 2, 5, 10 };
			foreach (var sizeOfBagger in sizeOfBaggers)
			{
				Console.WriteLine("Running with SizeOfBaggers {0}", sizeOfBagger);
				// Run the algorithm with different tree depths
				for (int treeDepth = 1; treeDepth <= 3; treeDepth++)
				{
					Console.WriteLine("Running with tree depth {0}", treeDepth);
					RunWithTreeLevels(trainingData, rnd, treeDepth, sizeOfBagger, testData);
				}
			}

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}

		private static void RunWithTreeLevels(ParserResults trainingData, Random rnd, int treeDepth, int sizeOfBaggers, ParserResults testData)
		{
			List<List<List<DataSetValue>>> dataSetValuesForBagging = new List<List<List<DataSetValue>>>();
			for (int i = 0; i < TotalSamplesForBiasAndVariance; i++)
			{
				// Two layer sampling
				List<DataSetValue> layer1Sampling = Bagging.ProduceDifferentDataSets(trainingData.Values, 1, rnd).Single();
				if (sizeOfBaggers == 1)
				{
					dataSetValuesForBagging.Add(new List<List<DataSetValue>>() {layer1Sampling});
				}
				else
				{
					dataSetValuesForBagging.Add(Bagging.ProduceDifferentDataSets(layer1Sampling, sizeOfBaggers, rnd));
				}
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

			//Console.WriteLine("Evaluating trees against test data...");
			double totalScoreAgainstTrainingData = 0;
			double totalScoreAgainstTestData = 0;
			foreach (List<DecisionTreeLevel> baggingSetOfTrees in listOfTreesToRunTestOn)
			{
				DecisionTreeScore scoreAgainstTrainingData = DecisionTreeScorer.ScoreWithTreeWithTestSet(baggingSetOfTrees, trainingData.Values);
				DecisionTreeScore scoreAgainstTestData = DecisionTreeScorer.ScoreWithTreeWithTestSet(baggingSetOfTrees, testData.Values);
				//score.PrintTotalScore();

				totalScoreAgainstTrainingData += scoreAgainstTrainingData.GetTotalScore();
				totalScoreAgainstTestData += scoreAgainstTestData.GetTotalScore();
			}
			totalScoreAgainstTrainingData = totalScoreAgainstTrainingData/listOfTreesToRunTestOn.Count;
			totalScoreAgainstTestData = totalScoreAgainstTestData / listOfTreesToRunTestOn.Count;

			double bias;
			double variance = BiasAndVarianceCalculator.CalculateBiasAndVariance(trainingData, listOfTreesToRunTestOn, out bias);

			Console.WriteLine("Variance: {0:0.00000}. Bias: {1:0.00000}. ScoreTraining : {2:0.00000}, ScoreTest : {3:0.00000}", variance, bias, totalScoreAgainstTrainingData, totalScoreAgainstTestData);
			//Console.WriteLine(bias);
			//Console.WriteLine(variance);
			//Console.WriteLine(totalScoreAgainstTrainingData);
			//Console.WriteLine(totalScoreAgainstTestData);
		}
	}
}
