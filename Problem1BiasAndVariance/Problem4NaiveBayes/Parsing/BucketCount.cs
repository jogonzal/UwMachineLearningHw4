namespace Problem4NaiveBayes.Parsing
{
	public class BucketCount
	{
		public uint OneCount { get; set; }
		public uint ZeroCount { get; set; }

		public BucketCount()
		{
			OneCount = 0;
			ZeroCount = 0;
		}

		public void Add(bool isOne)
		{
			if (isOne)
			{
				OneCount++;
			}
			else
			{
				ZeroCount++;
			}
		}
	}
}
