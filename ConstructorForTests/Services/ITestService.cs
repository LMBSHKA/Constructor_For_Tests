using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Services
{
	public interface ITestService
	{
		Task<IEnumerable<StatisticDto>> GetStatistic(StatisticFilterDto statisticFilter,
			int pageNumber, string curatorId);
		Task<SendTestDTO?> GetTest(Guid testId, bool isCurator);
		Task<TestForEditingDto> GetTestForEditing(Guid testId);
		string CalculateTimer(int timerInSeconds, string startTimer);
		Task<List<Test>> GetAllTests(string curatorId);
		Task<bool> CreateTest(CreateTestDto createTestData, string curatorId);
		Task SetCorrectAnswersForQuestions(List<BaseQuestionDto> listQuestions);
	}
}
