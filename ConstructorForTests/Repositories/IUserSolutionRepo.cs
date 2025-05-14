using ConstructorForTests.Dtos;

namespace ConstructorForTests.Repositories
{
	public interface IUserSolutionRepo
	{
		Task<decimal> CheckUserAnswers(List<UserAnswersDto> userSolution, Guid testId);
	}
}
