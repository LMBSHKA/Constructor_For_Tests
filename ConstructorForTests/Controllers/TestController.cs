using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using ConstructorForTests.Services;
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
		private readonly ITestService _testService;

		public TestController(ITestRepo testRepo, ITestService testService)
		{
			_testService = testService;
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
		/// Если отправит запрос куратор, то ему вернется тест, даже если он не активен
		/// </summary>
		/// <returns></returns>
		/// <param name="id">Указывать id теста</param>
		/// <response code="200" header="Remaining-Time">Успешное выполнение</response>
		/// <response code="404">Тест не найден</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpGet("UserGetTest/{id}")]
		public async Task<IActionResult> GetTestById(Guid id)
		{
			GetTestDTO? test;
			if (HttpContext.Session.GetString("CuratorId") == null)
				test = await _testService.GetTest(id, false);

			else
				test = await _testService.GetTest(id, true);

			if (test == null)
				return NotFound();

			if (HttpContext.Session.GetString("StartTime") == null)
			{
				var convertedTimer = TimeSpan.FromSeconds(Convert.ToDouble(test.TimerInSeconds));
				HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Remaining-Time", convertedTimer.ToString()));
				HttpContext.Session.SetString("StartTime", DateTime.Now.ToLongTimeString());
			}

			else
			{
				var startTimer = HttpContext.Session.GetString("StartTime");
				var remainingTime = _testService.CalculateTimer(test.TimerInSeconds!, startTimer!);
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
		///          "messageAboutPassing": "string",
		///          "failureMessage": "string",
		///          "timerInSeconds": 120,
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
			var curatorId = HttpContext.Session.GetString("CuratorId");
			if (await _testRepo.CreateTest(createTestData, curatorId))
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