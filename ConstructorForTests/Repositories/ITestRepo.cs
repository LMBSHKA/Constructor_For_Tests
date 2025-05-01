using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	public interface ITestRepo
	{
		Task<IEnumerable<Test>> GetAllTests();
		Task<bool> CreateTest(CreateTestDto createTestData);
		Task<bool> UpdateTest(Guid id, Test updateTestData);
		Task<GetTestDTO?> GetTestById(Guid id);
	}
}
