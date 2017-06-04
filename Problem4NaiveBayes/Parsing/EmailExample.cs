using System.Collections.Generic;

namespace Homework3.Parsing
{
	public class WordInEmail
	{
		public string Word { get; }
		public uint OccurrenceCount { get; }

		public WordInEmail(string word, uint occurrenceCount)
		{
			Word = word;
			OccurrenceCount = occurrenceCount;
		}
	}

	public class EmailExample
	{
		public string EmailId { get; }
		public bool IsSpam { get; }
		public IDictionary<string, uint> WordsInEmail { get; }

		public EmailExample(string emailId, bool isSpam, int wordCount)
		{
			EmailId = emailId;
			IsSpam = isSpam;
			WordsInEmail = new Dictionary<string, uint>(wordCount);
		}
	}
}
