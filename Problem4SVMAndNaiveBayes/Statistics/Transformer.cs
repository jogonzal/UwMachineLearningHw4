using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem1BiasAndVariance.Statistics
{
	public static class Transformer
	{
		public static double BoolToDouble(bool input)
		{
			return input ? 1.0 : 0.0;
		}
	}
}
