using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Problem1BiasAndVariance.DataSet;
using Problem1BiasAndVariance.DecisionTreeClasses;

namespace Problem1BiasAndVariance
{
	class Program
	{
		private const double ChiTestLimit = 0;
		public static int PercentilesToBreakIn = 3;

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

			Console.WriteLine("Validating data set");
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			List<List<List<DataSetValue>>> dataSetValuesForBagging = new List<List<List<DataSetValue>>>()
			{
				Bagging.ProduceDifferentDataSets(trainingData.Values, 1, rnd),
				//Bagging.ProduceDifferentDataSets(trainingData.Values, 3, rnd),
				//Bagging.ProduceDifferentDataSets(trainingData.Values, 5, rnd),
				//Bagging.ProduceDifferentDataSets(trainingData.Values, 10, rnd),
				//Bagging.ProduceDifferentDataSets(trainingData.Values, 20, rnd),
			};

			// Initialize the required trees
			List<List<DecisionTreeLevel>> listOfTreesToRunTestOn = new List<List<DecisionTreeLevel>>();
			foreach (var dataSetForBagging in dataSetValuesForBagging)
			{
				listOfTreesToRunTestOn.Add(dataSetForBagging.Select(x => new DecisionTreeLevel(ChiTestLimit, trainingData.Attributes, x)).ToList());
			}

			Console.WriteLine("Runnind D3 on all trees in parallel...");
			Parallel.ForEach(listOfTreesToRunTestOn.SelectMany(s => s), l => l.D3());

			Console.WriteLine("Deleting unecessary nodes...");
			Parallel.ForEach(listOfTreesToRunTestOn.SelectMany(s => s), l => l.TrimTree());

			Console.WriteLine("Getting test data set...");
			ParserResults testData = ParserUtils.ParseData(TestSetPath);

			Console.WriteLine("Evaluating trees against test data...");
			foreach (List<DecisionTreeLevel> baggingSetOfTrees in listOfTreesToRunTestOn)
			{
				DecisionTreeScore score = DecisionTreeScorer.ScoreWithTreeWithTestSet(baggingSetOfTrees, testData.Values);
				score.PrintTotalScore();
			}

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}
	}
}
