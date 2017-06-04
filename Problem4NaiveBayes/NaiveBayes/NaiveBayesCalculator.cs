using System;
using System.Collections.Generic;
using System.Linq;
using Problem4NaiveBayes.Parsing;

namespace Problem4NaiveBayes.NaiveBayes
{
	public static class NaiveBayesCalculator
	{
		//public static double ObtainProbabilityOfSpam(IDictionary<string, uint> testWordsInEmail, Dictionary<string, WordCount> trainingEmails)
		//{
		//	double probabilitySpam = 0;
		//	var totalWords = testWordsInEmail.Sum(w => w.Value);
		//	foreach (var u in testWordsInEmail)
		//	{
		//		double wordWeight = 1.0 * u.Value / totalWords;

		//		// Count all the times this word was spam
		//		WordCount wordCount;
		//		if (!trainingEmails.TryGetValue(u.Key, out wordCount))
		//		{
		//			// If we've never sen the word, then it's 50/50
		//			wordCount = new WordCount();
		//			wordCount.Add(false, 1);
		//			wordCount.Add(true, 1);
		//		}

		//		double probabilityOfWordBeingSpam = 1.0 * wordCount.ZeroCount/(wordCount.ZeroCount + wordCount.OneCount);

		//		probabilitySpam += wordWeight*probabilityOfWordBeingSpam;
		//	}

		//	return probabilitySpam;
		//}

		public static Tuple<double, double> ObtainProbabilityOfZeroAndOne(List<int> testValues, Dictionary<string, BucketCount> trainingEmails, double totalProbabilityOne)
		{
			double probabilityZero = 0, probabilityOne = 0;
			double totalProbabilityZero = 1 - totalProbabilityOne;
			for (int index = 0; index < testValues.Count; index++)
			{
				var testValue = testValues[index];
				string key = NaiveBayesDataTransform.BuildKey(index, testValue);
				// Count all the times this word was spam
				BucketCount bucketCount;
				if (!trainingEmails.TryGetValue(key, out bucketCount))
				{
					// laplace smoothing
					bucketCount = new BucketCount();
				}

				const double smoothingNum = 0.1; // Feel free to change this #

				double probabilityOfExampleBeingZero = (1.0*bucketCount.ZeroCount + smoothingNum)/
									(bucketCount.ZeroCount + bucketCount.OneCount + smoothingNum);
				double probabilityOfExampleBeingOne = (1.0*bucketCount.OneCount + smoothingNum)/
									(bucketCount.ZeroCount + bucketCount.OneCount + smoothingNum);

				probabilityZero += Math.Log(probabilityOfExampleBeingZero);
				probabilityOne += Math.Log(probabilityOfExampleBeingOne);
			}

			probabilityZero += Math.Log(totalProbabilityZero);
			probabilityOne += Math.Log(totalProbabilityOne);

			return new Tuple<double, double>(probabilityZero, probabilityOne);
		}
	}
}
