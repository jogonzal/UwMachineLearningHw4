using System;
using System.Collections.Generic;

namespace Problem1BiasAndVariance.DataSet
{
	public static class AttributeParser
	{
		public static List<DataSetAttribute> ParseAttributes(string dataSetAsString)
		{
			int indexOfData = dataSetAsString.IndexOf("@data");
			int indexOfAttributes = dataSetAsString.IndexOf("@attribute");

			string attributesString = dataSetAsString.Substring(indexOfAttributes, indexOfData - indexOfAttributes);

			var listOfAttributesAsString = attributesString.Split('\n');
			List<DataSetAttribute> attributes = new List<DataSetAttribute>(listOfAttributesAsString.Length);
			for (int index = 0; index < listOfAttributesAsString.Length; index++)
			{
				var attributeAsString = listOfAttributesAsString[index];
				if (string.IsNullOrEmpty(attributeAsString))
				{
					continue;
				}

				int indexOfFirstSpace = attributeAsString.IndexOf(" ");
				int indexOfStartOfEnums = attributeAsString.IndexOf(" {");
				int indexOfEndOfEnums = attributeAsString.IndexOf("}");

				string name = attributeAsString.Substring(indexOfFirstSpace + 1, indexOfStartOfEnums - (indexOfFirstSpace + 1));
				string enums = attributeAsString.Substring(indexOfStartOfEnums + 2, (indexOfEndOfEnums) - (indexOfStartOfEnums + 2));

				var enumValues = enums.Split(',');
				var hashSet = new HashSet<string>();
				foreach (var enumValue in enumValues)
				{
					if (!hashSet.Add(enumValue))
					{
						throw new InvalidOperationException();
					}
				}

				var attribute = new DataSetAttribute(name, hashSet, index);
				attributes.Add(attribute);
			}

			// Remove the first attribute, as it is the result
			attributes.RemoveAt(0);

			return attributes;
		}
	}
}
