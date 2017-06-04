using System;
using System.Collections.Generic;
using Problem4SVM.DataSet;

namespace Problem4SVM
{
	public static class Bagging
	{
		public static List<List<DataSetValue>> ProduceDifferentDataSets(List<DataSetValue> trainingDataValues, int count, Random rnd)
		{
			List<List<DataSetValue>> copyDataSets = new List<List<DataSetValue>>(count);
			for (int i = 0; i < count; i++)
			{
				List<DataSetValue> copy = new List<DataSetValue>(trainingDataValues.Count);
				for (int k = 0; k < trainingDataValues.Count; k++)
				{
					// "steal" a random item
					int r = rnd.Next(trainingDataValues.Count);
					copy.Add(trainingDataValues[r]);
				}

				copyDataSets.Add(copy);
			}
			return copyDataSets;
		}
	}
}
