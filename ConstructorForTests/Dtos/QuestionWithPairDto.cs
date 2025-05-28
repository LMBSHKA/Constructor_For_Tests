using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class QuestionWithPairDto : BaseQuestionDto
	{
		public AllAnswerDto Answers { get; set; }

		public QuestionWithPairDto(Question question, AllAnswerDto answers)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			Answers = answers;
			Type = question.Type;
			Mark = question.Mark;
			Order = question.Order;
		}
	}
}
