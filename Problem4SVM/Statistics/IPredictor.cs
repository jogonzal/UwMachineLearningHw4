using Problem4SVM.DataSet;

namespace Problem4NaiveBayes.Statistics
{
	public interface IPredictor
	{
		bool CalculatePrediction(DataSetValue trainingDataValue);
	}
}
