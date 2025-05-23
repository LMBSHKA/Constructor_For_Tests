using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	/// <summary>
	/// Endpoint-ы для операций над тестами
	/// </summary>
	[ApiController]
	[Route("api/v1/operationsOnTest")]
	public class TestController : ControllerBase
	{
		private readonly ITestRepo _testRepo;

		public TestController(ITestRepo testRepo)
		{
			_testRepo = testRepo;
		}

		/// <summary>
		/// Получение всех тестов
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpGet("GetAllTests")]
		public async Task<IActionResult> GetAllTests()
		{
			return Ok(await _testRepo.GetAllTests());
		}

		/// <summary>
		/// Получение теста по его идентификатору
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="404">Тест не найден</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpGet("GetTest/{id}")]
		public async Task<IActionResult> GetTestById(Guid id)
		{
			var test = await _testRepo.GetTestById(id, HttpContext.Session);

			if (test == null)
				return NotFound();

			return Ok(test);
		}

		/// <summary>
		/// Создание теста
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpPost("CreateTest")]
		public async Task<IActionResult> CreateTest([FromBody] CreateTestDto createTestData)
		{
			
			if (await _testRepo.CreateTest(createTestData))
				return Ok();

			return BadRequest("Something went wrong");
		}

		/// <summary>
		/// Неготово(Еще будет доделываться)
		/// Обновление данных теста
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpPut("Update/{id}")]
		public async Task<IActionResult> UpdateTest(Guid id, [FromBody] Test updateTestData)
		{
			if (await _testRepo.UpdateTest(id, updateTestData))
				return Ok();

			return BadRequest("Something went wrong");
		}

		/// <summary>
		/// Неготово(Еще будет доделываться)
		/// Получение данных для статистики по тестам
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpGet("GetStatistic")]
		public async Task<IActionResult> GetStatistic()
		{
			return Ok(await _testRepo.GetStatistic());
		}
	}
}