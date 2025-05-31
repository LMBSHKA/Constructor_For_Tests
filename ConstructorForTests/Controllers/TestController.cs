using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
			Console.WriteLine(HttpContext.Session.GetString("Time"));
			var listTests = await _testRepo.GetAllTests();
			var testsByUserId = listTests.Where(x => x.UserId == HttpContext.Session.GetString("CuratorId"));
			return Ok(testsByUserId);
		}

		/// <summary>
		/// Получение теста по его идентификатору
		/// </summary>
		/// <returns></returns>
		/// <param name="id">Указывать id теста</param>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="404">Тест не найден</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpGet("GetTest/{id}")]
		public async Task<IActionResult> UserGetTestById(Guid id)
		{
			var test = await _testRepo.GetTestById(id, HttpContext.Session);

			if (test == null)
				return NotFound();

			if (HttpContext.Session.GetString("StartTime") == null)
				HttpContext.Session.SetString("StartTime", DateTime.Now.ToLongTimeString());

			else
			{
				var startTimer = HttpContext.Session.GetString("StartTime");
				var passedTime = TimeSpan.Parse(DateTime.Now.ToLongTimeString()).Subtract(TimeSpan.Parse(startTimer!));
				var convertedTimer = TimeSpan.FromSeconds(Convert.ToDouble(test.TimerInSeconds));
				var remainingTime = convertedTimer.Subtract(passedTime).ToString();
				HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Remaining-Time", remainingTime));
			}

			return Ok(test);
		}

		/// <summary>
		/// Создание теста
		/// Только авторизованные пользователи
		/// </summary>
		/// <remarks>
		/// Note в questions указывать только один тип ответа все три не нужно, а если ручной ввод то ничего не указывать
		/// 
		/// Type 
		///     {
		///         Null = 0,
		///         SingleAnswer = 1,
		///         MultiplyAnswer = 2,
		///         MatchingPair = 3,
		///         DetailedAnswer = 4
		///     }
		/// 
		///     POST /Todo
		///     {
		///          "title": "new test",
		///          "startAt": "2025-05-23",
		///          "endAt": "2025-05-26",
		///          "scoreToPass": 1,
		///          "questions": 
		///          [
		///              {
		///                  "questionText": "answer",
		///                  "type": 0,
		///                  "createAnswer": 
		///                  {
		///                      "textAnswer": "answer",
		///                      "multipleAnswer": 
		///                      [
		///		                     "answer1", "answer2"
		///		                 ],
		///                      "matchingPairs": 
		///                      {
		///                          "pair1": "pair1",
		///                          "pair2": "pair2",
		///                          "pair3": "pair3"
		///                      }
		///                  }
		///              }
		///          ]
		///     }
		/// </remarks>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpPost("CreateTest")]
		public async Task<IActionResult> CreateTest([FromBody] CreateTestDto createTestData)
		{
			
			if (await _testRepo.CreateTest(createTestData, HttpContext.Session))
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