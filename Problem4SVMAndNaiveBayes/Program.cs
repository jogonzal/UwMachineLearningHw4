using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibSVMsharp;
using LibSVMsharp.Helpers;
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
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath, convertContinuousValues:false);
			Console.WriteLine("Validating training set");
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			Console.WriteLine("Getting test set...");
			ParserResults testData = ParserUtils.ParseData(TestSetPath, trainingData.Attributes, convertContinuousValues:false);
			Console.WriteLine("Validating test set");
			DataSetCleaner.ValidateDataSet(testData.Attributes, testData.Values);

			const string svmTrainingPath = "svmtraining.txt";
			const string svmTestPath = "svmtest.txt";
			LibSvmConverter.ConvertToLibSvm(trainingData, svmTrainingPath);
			LibSvmConverter.ConvertToLibSvm(testData, svmTestPath);

			SVMProblem problem = SVMProblemHelper.Load(svmTrainingPath);
			SVMProblem testProblem = SVMProblemHelper.Load(svmTestPath);

			SVMKernelType kernel = SVMKernelType.LINEAR;

			Console.WriteLine($"Using kernel {kernel}");

			SVMParameter parameter = new SVMParameter
			{
				Type = SVMType.C_SVC,
				Kernel = kernel,
				C = 1,
				Gamma = 1
			};

			SVMModel model = SVM.Train(problem, parameter);

			double []target = new double[testProblem.Length];
			for (int i = 0; i < testProblem.Length; i++)
				target[i] = SVM.Predict(model, testProblem.X[i]);

			double accuracy = SVMHelper.EvaluateClassificationProblem(testProblem, target);

			Console.WriteLine($"SVM Accuracy is {accuracy}");

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}
	}
}
