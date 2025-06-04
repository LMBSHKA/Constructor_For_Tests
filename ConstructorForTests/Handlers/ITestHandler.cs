using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Handlers
{
	public interface ITestHandler
	{
		void GetTestById(List<BaseQuestionDto> listGetQuestions, List<Question> questions);
		void CreateStatisticDto(StatisticFilterDto statisticFilter, List<StatisticDto> statistics,
			User user, Test test, TestResult testResult);
	}
}
