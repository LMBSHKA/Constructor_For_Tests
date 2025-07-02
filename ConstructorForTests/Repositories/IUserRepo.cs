using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	public interface IUserRepo
	{
		Task<decimal> CheckUserAnswers(UserSolutionDto userSolution, Guid testId, Guid userId);
		Task<Guid> CreateUser(User newUser);
		bool CheckPassage(Test test, decimal score);
		public User? GetUserById(Guid userId);
		IQueryable<User> GetAllUsers();
	}
}
