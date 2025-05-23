using ConstructorForTests.Dtos;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	/// <summary>
	/// Endpoint-ы для проходящего тесты
	/// </summary>
	[ApiController]
	[Route("api/v1/UserSolution")]
	public class UserController : ControllerBase
	{
		private readonly IUserRepo _userSolutionRepo;

		public UserController(IUserRepo userSolutionRepo)
		{
			_userSolutionRepo = userSolutionRepo;
		}

		/// <summary>
		/// Получение ответов на тест от прохдящего
		/// </summary>
		/// <remarks>
		/// Note в answer указывать что-то одно все типы ответов не нужно
		/// 
		///     POST /Todo
		///     {
		///         "firstName": "Ivan",
		///         "secondName": "Ivanov",
		///         "patronymic": "Ivanovich",
		///         "email": "email@email.com",
		///         "answers": 
		///         [
		///             {
		///                 "questionId" "0e7ad584-7788-4ab1-95a6-ca0a5b444cbb",
		///                 "textAnswer": "answer",
		///                 "multipleAnswer": 
		///                 [
		///		                "answer1", "answer2"
		///		            ],
		///                 "matchingPairs": 
		///                 {
		///                     "pair1": "pair1",
		///                     "pair2": "pair2",
		///                     "pair3": "pair3"
		///                 }
		///             }
		///         ]
		///     }
		/// </remarks>
		/// <param name="testId">указывать id теста</param>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpPost("{testId}")]
		public async Task<IActionResult> AcceptUserSolution(UserSolutionDto userSolution, Guid testId)
		{
			var newUser = new Models.User(
				userSolution.FirstName,
				userSolution.SecondName,
				userSolution.Patronymic,
				userSolution.Email);
			var userId = await _userSolutionRepo.CreateUser(newUser);

			return Ok(await _userSolutionRepo.CheckUserAnswers(userSolution, testId, userId));
		}
	}
}
