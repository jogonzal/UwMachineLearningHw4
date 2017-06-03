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

		public static Tuple<double, double> ObtainProbabilityOfSpam(IDictionary<string, uint> testWordsInEmail, Dictionary<string, BucketCount> trainingEmails, double totalProbabilitySpam, int totalNumberOfWords)
		{
			double probabilitySpam = 0, probabilityHam = 0;
			double totalProbabilityHam = 1 - totalProbabilitySpam;
			var totalWords = testWordsInEmail.Sum(w => w.Value);
			foreach (var u in testWordsInEmail)
			{
				double wordWeight = 1.0 * u.Value / totalWords;

				// Count all the times this word was spam
				BucketCount bucketCount;
				if (!trainingEmails.TryGetValue(u.Key, out bucketCount))
				{
					// laplace smoothing
					bucketCount = new BucketCount();
				}

				const double smoothingNum = 100; // Feel free to change this #

				double probabilityOfWordBeingSpam = (1.0 * bucketCount.ZeroCount + smoothingNum) / (bucketCount.ZeroCount + bucketCount.OneCount + smoothingNum);
				double probabilityOfWordBeingHam = (1.0 * bucketCount.OneCount + smoothingNum) / (bucketCount.ZeroCount + bucketCount.OneCount + smoothingNum);

				probabilitySpam += Math.Log(probabilityOfWordBeingSpam);
				probabilityHam += Math.Log(probabilityOfWordBeingHam);
			}

			probabilitySpam += Math.Log(totalProbabilitySpam);
			probabilityHam += Math.Log(totalProbabilityHam);

			return new Tuple<double, double>(probabilitySpam, probabilityHam);
		}
	}
}
