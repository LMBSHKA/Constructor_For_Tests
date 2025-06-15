using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	public interface ITestRepo
	{
		IQueryable<Test> GetAllTests();
		Task CreateTestInfo(Test test);
		Task AddQuestion(Question newQuestion);
		Task AddAnswer(Answer newAnswer);
		Task AddSingleSolutionForMultipleAnswer(MultipleChoice newSingleAnswer);
		Task AddPairAnswer(MatchingPair pairAnswer);
		Task<bool> UpdateTest(Guid id, Test updateTestData);
		Task<Test?> GetTestInfoById(Guid id);
		IOrderedQueryable<Question> GetTestQuestion(Guid testId);
		Task<List<StatisticDto>> GetStatistic(StatisticFilterDto statisticFilter, int pageNumber);
		Task<int> DeleteTest(Guid testId);
		Task<int> UpdateResult(Guid testId, List<ManualCheckDto> manualCheckData);
		Task<List<SendTestToCheckDto>> CreateTestDtoToCheck(Guid testId);
		Task SaveChecnhesAsync();
	}
}
