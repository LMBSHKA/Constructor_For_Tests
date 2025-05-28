using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class QuestionWithOptionsDto : BaseQuestionDto
	{
		public AllAnswerDto Answers { get; set; }

		public QuestionWithOptionsDto(Question question, AllAnswerDto answers)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			Answers = answers;
			Type = question.Type;
		}
	}
}
