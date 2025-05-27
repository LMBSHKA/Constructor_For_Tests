using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class QuestionWithOptionsDto : BaseQuestionDto
	{
		public string[] AnswerOptions { get; set; }

		public QuestionWithOptionsDto(Question question, string[] answerOptions)
		{
			Id = question.Id;
			TestId = question.TestId;
			QuestionText = question.QuestionText;
			AnswerOptions = answerOptions;
			Type = question.Type;
			Mark = question.Mark;
			Order = question.Order;
		}
	}
}
