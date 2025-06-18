using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class QuestionWithPairDto : BaseQuestionDto
	{
		public AllAnswerDto Answers { get; set; }
		public AllAnswerDto CorrectAnswers { get; set; }

		public QuestionWithPairDto() { }

		public QuestionWithPairDto(Question question, AllAnswerDto answers)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			Answers = answers;
			Type = question.Type;
		}
	}
}
