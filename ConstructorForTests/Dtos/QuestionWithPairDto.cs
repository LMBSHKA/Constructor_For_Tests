using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class QuestionWithPairDto : BaseQuestionDto
	{
		public string[] PairKey { get; set; }
		public string[] PairValue { get; set; }

		public QuestionWithPairDto(Question question, string[] pairKey, string[] pairValue)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			PairKey = pairKey;
			PairValue = pairValue;
			Type = question.Type;
			Mark = question.Mark;
			Order = question.Order;
		}
	}
}
