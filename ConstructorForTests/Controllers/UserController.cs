using ConstructorForTests.Dtos;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	[ApiController]
	[Route("api/v1/UserSolution")]
	public class UserController : ControllerBase
	{
		private readonly IUserRepo _userSolutionRepo;

		public UserController(IUserRepo userSolutionRepo)
		{
			_userSolutionRepo = userSolutionRepo;
		}

		[HttpPost("{testId}")]
		public async Task<IActionResult> AcceptUserSolution(UserSolutionDto userSolution, Guid testId)
		{
			return Ok(await _userSolutionRepo.CheckUserAnswers(userSolution, testId));
		}
	}
}
