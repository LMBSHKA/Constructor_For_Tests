using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Handlers
{
	public interface ITestHandler
	{
		void CreateStatisticDto(StatisticFilterDto statisticFilter, List<StatisticDto> statistics,
			User user, Test test, TestResult testResult);
		void CreateListToSend(List<SendTestToCheckDto> listTestToSend, IQueryable<UserAnswer> userAnswers, Question question);
	}
}
