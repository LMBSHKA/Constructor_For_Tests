using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Handlers
{
	public class TestHandler : ITestHandler
	{
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

		public void CreateListToSend(List<SendTestToCheckDto> listTestToSend, 
			IQueryable<UserAnswer> userAnswers, Question question)
		{
			foreach (var userAnswer in userAnswers)
			{
				var testToCheck = listTestToSend.FirstOrDefault(x => x.Userid == userAnswer.UserId);
				if (testToCheck == null)
				{
					var sendToCheck = new SendTestToCheckDto(userAnswer.UserId,
						[new QuestionToCheckDto(question.Id, question.QuestionText, userAnswer.Text)]);
					listTestToSend.Add(sendToCheck);
				}
				else
				{
					testToCheck.QuestionToCheckDtos.Add(new QuestionToCheckDto(question.Id,
						question.QuestionText, userAnswer.Text));
				}
			}

		}
	}
}
