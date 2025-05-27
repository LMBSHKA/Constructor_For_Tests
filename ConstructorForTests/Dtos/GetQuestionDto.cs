using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class GetQuestionDto : Question
	{
		public string[]? AnswerOptions { get; set; } = null;
		public string[]? PairKey { get; set; } = null;
		public string[]? PairValue { get; set; } = null;

		public GetQuestionDto(Question question, string[] answerOptions)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			AnswerOptions = answerOptions;
			Type = question.Type;
			Mark = question.Mark;
			Order = question.Order;

		}

		public GetQuestionDto(Question question, string[] pairKey, string[] pairValue)
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
		public GetQuestionDto(Question question)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			Type = question.Type;
			Mark = question.Mark;
			Order = question.Order;
		}
	}
}
