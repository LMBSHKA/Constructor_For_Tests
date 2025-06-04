using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

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
					questionDto = new QuestionWithOptionsDto(question, new AllAnswerDto(question.AnswerOptions.Split(' ')));
				}

				else if (!string.IsNullOrEmpty(question.PairKey) && !string.IsNullOrEmpty(question.PairValue))
				{
					questionDto = new QuestionWithPairDto(question, new AllAnswerDto(question.PairKey.Split(' '), question.PairValue.Split(' ')));
				}

				else
				{
					questionDto = new SimpleQuestionDto(question);
				}

				listGetQuestions.Add(questionDto);
			}
		}

		public void CreateStatisticDto(StatisticFilterDto statisticFilter, List<StatisticDto> statistics,
			User user, Test test, TestResult testResult)
		{
			var fullName = string.Join(' ', [user.SecondName, user.FirstName, user.Patronymic]);

			if (statisticFilter.FullName != null && statisticFilter.FullName.Equals(fullName) || statisticFilter.FullName == "")
			{
				var statistic = new StatisticDto(
					string.Join(' ', [user.FirstName, user.SecondName, user.Patronymic]),
					user.Email,
					test.Title,
					testResult.IsPassed,
					testResult.TotalScore);

				statistics.Add(statistic);
			}
		}
	}
}
