using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	public interface ITestRepo
	{
		Task<List<Test>> GetAllTests();
		Task<bool> CreateTest(CreateTestDto createTestData, ISession session);
		Task<bool> UpdateTest(Guid id, Test updateTestData);
		Task<GetTestDTO?> GetTestById(Guid id, ISession session);
		Task<List<StatisticDto>> GetStatistic();
	}
}
