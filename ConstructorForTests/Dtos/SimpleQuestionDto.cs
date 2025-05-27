using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class SimpleQuestionDto : BaseQuestionDto
	{
		public SimpleQuestionDto(Question question)
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
