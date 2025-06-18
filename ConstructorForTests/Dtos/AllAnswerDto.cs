using Microsoft.Extensions.Logging.Abstractions;

namespace ConstructorForTests.Dtos
{
	public class AllAnswerDto
	{
		public string[]? AnswerOptions { get; set; } = null;
		public string[]? PairKey { get; set; } = null;
		public string[]? PairValue { get; set; } = null;

		public AllAnswerDto(string[]? answerOptions)
		{
			AnswerOptions = answerOptions;
		}

		public AllAnswerDto(string[]? pairKey, string[]? pairValue)
		{
			PairKey = pairKey;
			PairValue = pairValue;
		}
	}
}
