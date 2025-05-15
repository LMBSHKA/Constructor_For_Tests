using ConstructorForTests.Dtos;

namespace ConstructorForTests.Repositories
{
	public interface IUserRepo
	{
		Task<decimal> CheckUserAnswers(UserSolutionDto userSolution, Guid testId);
	}
}
