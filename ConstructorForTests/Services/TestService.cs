using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Services
{
	public class TestService : ITestService
	{
		private readonly ITestRepo _testRepo;

		public TestService(ITestRepo testRepo)
		{
			_testRepo = testRepo;
		}

		public async Task<List<Test>> GetAllTests(string curatorId)
		{
			var allTests = _testRepo.GetAllTests();
			var testsByUserId = await allTests
				.Where(x => x.UserId == curatorId)
				.ToListAsync();

			return testsByUserId;
		}

		public async Task<SendTestDTO?> GetTest(Guid testId, bool isCurator)
		{
			var test = await _testRepo.GetTestInfoById(testId);

			if (test == null || (test.IsActive == false && isCurator == false))
				return null;

			var questions = await _testRepo.GetTestQuestion(testId)
				.ToListAsync();

			var listGetQuestions = new List<BaseQuestionDto>();

			CreateListBaseQuestionDto(listGetQuestions, questions);

			return new SendTestDTO(test, listGetQuestions);
		}

		private static void CreateListBaseQuestionDto(List<BaseQuestionDto> listGetQuestions, List<Question> questions)
		{
			foreach (var question in questions)
			{
				BaseQuestionDto questionDto;

				if (!string.IsNullOrEmpty(question.AnswerOptions))
				{
					questionDto = new QuestionWithOptionsDto(question, new AllAnswerDto(question.AnswerOptions.Split(' ')));
				}

				else if (!string.IsNullOrEmpty(question.PairKey) && !string.IsNullOrEmpty(question.PairValue))
				{
					questionDto = new QuestionWithPairDto(question, 
						new AllAnswerDto(question.PairKey.Split(' '), question.PairValue.Split(' ')));
				}

				else
				{
					questionDto = new SimpleQuestionDto(question);
				}

				listGetQuestions.Add(questionDto);
			}
		}

		public string CalculateTimer(int timerInSeconds, string startTimer)
		{
			var passedTime = TimeSpan.Parse(DateTime.Now.ToLongTimeString()).Subtract(TimeSpan.Parse(startTimer!));
			var convertedTimer = TimeSpan.FromSeconds(Convert.ToDouble(timerInSeconds));
			var remainingTime = convertedTimer.Subtract(passedTime).ToString();
			var remainingTimeSeconds = TimeSpan.Parse(remainingTime).TotalSeconds.ToString();

			return remainingTimeSeconds;
		}
	}
}
