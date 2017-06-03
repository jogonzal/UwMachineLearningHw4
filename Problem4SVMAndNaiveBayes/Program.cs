using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Problem4SVMAndNaiveBayes.DataSet;

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

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}
	}
}
