using ConstructorForTests.Dtos;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	[ApiController]
	[Route("api/v1/UserSolution")]
	public class UserSolutionsController : ControllerBase
	{
		private readonly IUserSolutionRepo _userSolutionRepo;

		public UserSolutionsController(IUserSolutionRepo userSolutionRepo)
		{
			_userSolutionRepo = userSolutionRepo;
		}

		[HttpPost("{testId}")]
		public async Task<IActionResult> AcceptUserSolution(List<UserAnswersDto> userSolution, Guid testId)
		{
			return Ok(await _userSolutionRepo.CheckUserAnswers(userSolution, testId));
		}
	}
}
