using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Problem4SVM.DataSet
{
	public static class LibSvmConverter
	{
		public static void ConvertToLibSvm(List<DataSetValue> values, string targetFile)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var row in values)
			{
				sb.Append(row.Output ? "1" : "0").Append(" ");
				for (int index = 0; index < row.Values.Count; index++)
				{
					var rowValue = row.Values[index];
					sb.Append(index + 1).Append(":").Append(rowValue).Append(" ");
				}
				sb.AppendLine();
			}
			string textToWrite = sb.ToString();
			File.WriteAllText(targetFile, textToWrite);
		}
	}
}
