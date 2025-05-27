using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Handlers
{
	public class TestHandler : ITestHandler
	{
		public void GetTestById(List<BaseQuestionDto> listGetQuestions, List<Question> questions)
		{
			foreach (var question in questions)
			{
				BaseQuestionDto questionDto;

				if (!string.IsNullOrEmpty(question.AnswerOptions))
				{
					questionDto = new QuestionWithOptionsDto(question, question.AnswerOptions.Split(' '));
				}

				else if (!string.IsNullOrEmpty(question.PairKey) && !string.IsNullOrEmpty(question.PairValue))
				{
					questionDto = new QuestionWithPairDto(question, question.PairKey.Split(' '), question.PairValue.Split(' '));
				}

				else
				{
					questionDto = new SimpleQuestionDto(question);
				}

				listGetQuestions.Add(questionDto);
			}
		}
	}
}
