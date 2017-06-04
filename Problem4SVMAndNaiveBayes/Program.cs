using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Problem1BiasAndVariance.Statistics;
using Problem4SVM.DataSet;

namespace Problem4SVM
{
	class Program
	{
		public static int PercentilesToBreakIn = 10;
		public static int TotalSamplesForBiasAndVariance = 3;

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
			ParserResults testData = ParserUtils.ParseData(TestSetPath, trainingData.Attributes, convertContinuousValues: false);
			Console.WriteLine("Validating test set");
			DataSetCleaner.ValidateDataSet(testData.Attributes, testData.Values);

			const string svmTrainingPath = "svmtraining";
			const string svmOutputPath = "output";

			List<int> kernelsToRunIn = new List<int>()
			{
				2, 3
			};

			string originalTrainingFile = "originalTraining.txt";
			LibSvmConverter.ConvertToLibSvm(trainingData.Values, "originalTraining.txt");

			// Run all kernels in parallel
			foreach (var kernel in kernelsToRunIn)
			{
				Enumerable.Range(0, TotalSamplesForBiasAndVariance).AsParallel().Select((i) =>
				{
					Console.WriteLine("Doing loop {0} for kernel {1}", i, kernel);

					var postFix = string.Format("K{0}-I{1}.txt", kernel, i);
					string trainingPath = svmTrainingPath + postFix;
					string trainingModelPath = trainingPath + ".model";
					string outputPath = svmOutputPath + postFix;

					List<List<DataSetValue>> differentTrainingData = Bagging.ProduceDifferentDataSets(trainingData.Values, 1, rnd);
					LibSvmConverter.ConvertToLibSvm(differentTrainingData.Single(), trainingPath);
					if (!File.Exists(trainingModelPath))
					{
						RunTrainingExe(trainingPath, kernel);
					}
					if (!File.Exists(outputPath))
					{
						RunEvaluateExe(originalTrainingFile, trainingModelPath, outputPath);
					}
					return 0;
				}).ToList();
			}

			foreach (var kernel in kernelsToRunIn)
			{
				List<List<bool>> allPredictions = new List<List<bool>>();

				for (int i = 0; i < TotalSamplesForBiasAndVariance; i++)
				{
					Console.WriteLine("Evaluating loop {0} for kernel {1}", i, kernel);
					var postFix = string.Format("K{0}-I{1}.txt", kernel, i);
					string trainingPath = svmTrainingPath + postFix;
					string outputPath = svmOutputPath + postFix;

					if (!File.Exists(trainingPath) || !File.Exists(outputPath))
					{
						continue;
					}

					List<bool> predictions = GetPredictionsFromOutputPath(outputPath);
					allPredictions.Add(predictions);
				}

				if (allPredictions.Count == 0)
				{
					Console.WriteLine("Not enough information to evaluate kernel {0}", kernel);
					continue;
				}

				double bias;
				double variance = BiasAndVarianceCalculator.CalculateBiasAndVariance(trainingData, allPredictions, out bias);
				Console.WriteLine("Bias:{0:0.00000} Variance:{1:0.00000}", bias, variance);
			}

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}

		private static List<bool> GetPredictionsFromOutputPath(string outputPath)
		{
			List<bool> predictions = new List<bool>();
			var lines = File.ReadAllLines(outputPath);
			foreach (string line in lines)
			{
				bool b = line == "1";
				predictions.Add(b);
			}
			return predictions;
		}

		private static void RunEvaluateExe(string testPath, string trainingModelPath, string outputPath)
		{
			RunExeAndGetOutput("svm-predict.exe", $"{testPath} {trainingModelPath} {outputPath}");
		}

		private static void RunTrainingExe(string trainingPath, int kernel)
		{
			RunExeAndGetOutput("svm-train.exe", $"-t {kernel} {trainingPath}");
		}

		private static string RunExeAndGetOutput(string exe, string args)
		{
			string processName =
				Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), exe);
			processName = processName.Substring(6); // THIS REMOVES THE "FILE://"

			Console.WriteLine($"Running {exe} {args}");

			ProcessStartInfo psi = new ProcessStartInfo(processName, args)
			{
				RedirectStandardOutput = true,
				RedirectStandardInput = true,
				CreateNoWindow = false,
				UseShellExecute = false
			};

			string outputString;

			using (var p = new Process())
			{
				p.StartInfo = psi;
				p.Start();

				using (var sr = new StreamReader(p.StandardOutput.BaseStream))
				{
					outputString = sr.ReadToEnd();
				}
			}

			Console.WriteLine($"Ran {exe} {args}");
			Console.WriteLine(outputString);

			return outputString;
		}
	}
}
