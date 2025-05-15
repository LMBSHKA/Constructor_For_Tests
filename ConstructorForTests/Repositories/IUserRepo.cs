using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	public interface IUserRepo
	{
		Task<decimal> CheckUserAnswers(UserSolutionDto userSolution, Guid testId, Guid userId);
		Task<Guid> CreateUser(User newUser);
	}
}
