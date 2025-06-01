using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	public interface ITestRepo
	{
		Task<List<Test>> GetAllTests();
		Task<bool> CreateTest(CreateTestDto createTestData, ISession session);
		Task<bool> UpdateTest(Guid id, Test updateTestData);
		Task<Test?> GetTestInfoById(Guid id);
		Task<List<Question>> GetTestQuestion(Guid testId);
		Task<List<StatisticDto>> GetStatistic();
	}
}
