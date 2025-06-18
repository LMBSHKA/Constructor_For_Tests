using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using ConstructorForTests.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			var curatorId = HttpContext.Session.GetString("CuratorId");
			var listTestsByCuratorId = await _testService.GetAllTests(curatorId!);

			return Ok(listTestsByCuratorId);
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
		[TypeFilter(typeof(SetTimerAttribute))]
		public async Task<IActionResult> GetTestById(Guid id)
		{
			var isCurator = HttpContext.Session.GetString("CuratorId") != null;
			var test = await _testService.GetTest(id, isCurator);

			if (test == null)
				return NotFound();

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
			if (await _testService.CreateTest(createTestData, curatorId!))
				return Ok();

			return BadRequest("Something went wrong");
		}

		/// <summary>
		/// Обновление данных теста
		/// Только авторизованные пользователи
		/// Передавать то же самое, что и при создании теста
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpPut("Update/{id}")]
		public async Task<IActionResult> UpdateTest(Guid id, [FromBody] CreateTestDto createTestDto)
		{
			var code = await _testRepo.DeleteTest(id);
			if (code == 404)
				return NotFound();

			var curatorId = HttpContext.Session.GetString("CuratorId");
			if (await _testService.CreateTest(createTestDto, curatorId!))
				return Ok();

			return BadRequest("Something went wrong");
		}

		/// <summary>
		/// Получение данных для статистики по тестам
		/// Есть фильтры и пагинация, должны передаваться в теле запроса
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpGet("GetStatistic")]
		public async Task<IActionResult> GetStatistic([FromQuery] StatisticFilterDto statisticFilter, 
			[FromQuery] int pageNumber = 1)
		{
			return Ok(await _testRepo.GetStatistic(statisticFilter, pageNumber));
		}

		/// <summary>
		/// Удаление теста
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpDelete("Delete/{testId}")]
		public async Task<IActionResult> DeleteTest(Guid testId)
		{
			var status = await _testRepo.DeleteTest(testId);
			if (status == 404)
				return NotFound();

			return Ok();
		}

		/// <summary>
		/// Принятие результатов ручной проверки
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpPost("ManualCheck/{testId}")]
		public async Task<IActionResult> SetManualCheck(Guid testId, List<ManualCheckDto> manualCheckData)
		{
			var status = await _testRepo.UpdateResult(testId, manualCheckData);
			if (status == 404)
				return NotFound();

			return Ok();
		}

		/// <summary>
		/// Получение всеех пользователей и их ответов в тесте для ручной проверки
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpGet("ManualCheck/{testId}")]
		public async Task<IActionResult> GetTestToCheck(Guid testId)
		{
			var listTestToSend = await _testRepo.CreateTestDtoToCheck(testId);
			return Ok(listTestToSend);
		}

		/// <summary>
		/// Получение теста и его ответов по его id для редактирования
		/// Только авторизованные пользователи
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Ошибка API(Переданы некорретные данные)</response>
		/// <response code="500">Ошибка сервера</response>
		[SessionAuthentication]
		[HttpGet("GetTest/Redactor{testId}")]
		public async Task<IActionResult> GetTestByIdForRedactor(Guid testId)
		{
			var test = await _testService.GetTest(testId, true);
			if (test == null)
				return NotFound();

			await _testService.SetCorrectAnswersForQuestions(test.Questions);

			return Ok(test);
		}
	}
}