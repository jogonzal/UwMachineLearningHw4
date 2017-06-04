using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Problem4NaiveBayes.DataSet;

namespace Problem4NaiveBayes.Statistics
{
	public interface IPredictor
	{
		bool CalculatePrediction(DataSetValue trainingDataValue);
	}
}
