using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Services
{
	/// <summary>
	/// Бизнесовая логика
	/// </summary>
	public class TestService : ITestService
	{
		private readonly ITestRepo _testRepo;
		private readonly IUserRepo _userRepo;
		private int PageSize { get; } = 20;

		public TestService(ITestRepo testRepo, IUserRepo userRepo)
		{
			_userRepo = userRepo;
			_testRepo = testRepo;
		}

		public async Task<IEnumerable<StatisticDto>> GetStatistic(StatisticFilterDto statisticFilter,
			int pageNumber, string curatorId)
		{
			var tests = await _testRepo.GetAllTests()
				.Where(x =>
					x.UserId == curatorId &&
					EF.Functions.Like(x.Title, $"%{statisticFilter.TestName}%"))
				.ToArrayAsync();

			var arrayTestResults = await SetFilterToTestResult(statisticFilter, tests);
			var statistics = await CreateListStatistic(statisticFilter, tests, arrayTestResults);
			var pagedItems = SetPagination(pageNumber, statistics);

			return pagedItems;
		}

		private async Task<List<StatisticDto>> CreateListStatistic(StatisticFilterDto statisticFilter,
			Test[] tests, TestResult[] arrayTestResults)
		{
			var statistics = new List<StatisticDto>();
			foreach (var testResult in arrayTestResults)
			{
				var test = tests
					.FirstOrDefault(x => x.Id == testResult.TestId);

				if (test != null)
				{
					var user = await SetFilterToUser(statisticFilter, testResult);
					if (user != null)
					{
						CreateStatisticDto(statisticFilter, statistics, user, test, testResult);
					}
				}
			}

			return statistics;
		}

		private IEnumerable<StatisticDto> SetPagination(int pageNumber, List<StatisticDto> listTestResults)
		{
			var startIndex = (pageNumber - 1) * PageSize;
			var pagedItems = listTestResults
				.Skip(startIndex)
				.Take(PageSize);

			return pagedItems;
		}

		private void CreateStatisticDto(StatisticFilterDto statisticFilter, List<StatisticDto> statistics,
			User user, Test test, TestResult testResult)
		{
			var fullName = string.Join(' ', [user.SecondName, user.FirstName, user.Patronymic]);

			if (statisticFilter.FullName != null && statisticFilter.FullName.Equals(fullName) || statisticFilter.FullName == "")
			{
				var statistic = new StatisticDto(
					string.Join(' ', [user.FirstName, user.SecondName, user.Patronymic]),
					user.Email,
					test.Title,
					testResult.IsPassed,
					testResult.TotalScore);

				statistics.Add(statistic);
			}
		}

		private async Task<User?> SetFilterToUser(StatisticFilterDto statisticFilter, TestResult testResult)
		{
			var userWithFilter = await _userRepo.GetAllUsers()
				.FirstOrDefaultAsync(x =>
				x.Id == testResult.UserId &&
				EF.Functions.Like(x.Email, $"%{statisticFilter.Email}%"));

			return userWithFilter;
		}

		private async Task<TestResult[]> SetFilterToTestResult(StatisticFilterDto statisticFilter, Test[] tests)
		{
			var arrayTestResults = await _testRepo.GetTestResult()
			.Where(stat =>
			tests.Select(x => x.Id).Contains(stat.TestId) &&
			EF.Functions.Like(stat.IsPassed.ToString(),
			$"%{(statisticFilter.Result == null ? "" : statisticFilter.Result)}%") &
			EF.Functions.Like(stat.TotalScore.ToString(),
			$"%{(statisticFilter.Score == -1 ? "" : statisticFilter.Score)}%"))
			.ToArrayAsync();

			return arrayTestResults;
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

		public async Task<TestForEditingDto> GetTestForEditing(Guid testId)
		{
			var test = await _testRepo.GetTestInfoById(testId);
			if (test == null && test.IsActive == true)
				return null;

			var questions = await _testRepo.GetTestQuestion(testId)
				.ToListAsync();

			var listGetQuestions = new List<BaseQuestionDto>();

			CreateListBaseQuestionDto(listGetQuestions, questions);

			return new TestForEditingDto(test, listGetQuestions);
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

		public async Task SetCorrectAnswersForQuestions(List<BaseQuestionDto> listQuestions)
		{
			foreach (var question in listQuestions)
			{
				var correctAnswer = await _testRepo.GetQuestionsCorrectAnswers(question.Id);
				var testQuestion = await _testRepo.GetTestQuestion(question.TestId)
					.FirstOrDefaultAsync(x => x.Id == question.Id);
				if (testQuestion != null && testQuestion.Mark > 0)
					question.Mark = testQuestion!.Mark;
				if (correctAnswer != null)
				{
					if (correctAnswer != null && question is SimpleQuestionDto simpleQuestion)
					{
						var answer = new AllAnswerDto([correctAnswer.TextAnswer]);
						simpleQuestion.CorrectAnswers = answer;
					}
					else if (correctAnswer != null && question is QuestionWithOptionsDto questionWithOptions)
					{
						var multipleAnswer = await _testRepo.GetMultipleChoice(correctAnswer.MultipleAnswerId)
							.Select(x => x.Answer)
							.ToArrayAsync();
						var answers = new AllAnswerDto(multipleAnswer!);
						questionWithOptions.CorrectAnswers = answers;
					}
					else if (correctAnswer != null && question is QuestionWithPairDto questionWithPair)
					{
						var pairs = await _testRepo.GetMatchingPair(correctAnswer.PairId)
							.ToArrayAsync();
						var pairKey = pairs.Select(x => x.PairKey).ToArray();
						var pairValue = pairs.Select(x => x.PairValue).ToArray();
						var answers = new AllAnswerDto(pairKey, pairValue);
						questionWithPair.CorrectAnswers = answers;
					}
				}
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

		public async Task<bool> CreateTest(CreateTestDto createTestData, string curatorId)
		{
			if (createTestData.Title == null)
				return false;

			var isActive = false;
			if (createTestData.StartAt <= DateTime.Now)
				isActive = true;

			var newTest = new Test(createTestData, isActive, false, curatorId!);
			await _testRepo.CreateTestInfo(newTest);
			await AddQuestions(createTestData.Questions!, newTest);
			await _testRepo.SaveChecnhesAsync();

			return true;
		}

		private async Task AddQuestions(List<CreateQuestionDTO> questions, Test test)
		{
			var order = 1;
			foreach (var question in questions)
			{
				var newQuestion = new Question(test.Id, question, order);
				CheckForManualCheck(test, question.Type);

				await _testRepo.AddQuestion(newQuestion);
				await AddAnswer(newQuestion.Id, question.CreateAnswer, test.Id);
				order++;
			}
		}

		private static void CheckForManualCheck(Test test, QuestionType questionType)
		{
			if (questionType == QuestionType.DetailedAnswer)
				test.ManualCheck = true;
		}

		private async Task AddAnswer(Guid questionId, AnswerDTO answer, Guid testId)
		{
			if (!string.IsNullOrEmpty(answer.TextAnswer))
			{
				var newAnswer = new Answer(questionId, Guid.Empty, Guid.Empty, answer.TextAnswer, testId);
				await _testRepo.AddAnswer(newAnswer);
			}

			else if (answer.MultipleAnswer.Count > 0)
			{
				await AddMultipleAnswer(answer.MultipleAnswer, testId, questionId);
			}

			else if (answer.MatchingPairs.Count > 0)
			{
				await AddPairAnswer(answer.MatchingPairs, testId, questionId);
			}
		}

		private async Task AddMultipleAnswer(List<string> multipleAnswers, Guid testId, Guid questionId)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(questionId, guid, Guid.Empty, string.Empty, testId);
			await _testRepo.AddAnswer(newAnswer);

			foreach (var singleAnswer in multipleAnswers)
			{
				var newSingleAnswer = new MultipleChoice(guid, singleAnswer);
				await _testRepo.AddSingleSolutionForMultipleAnswer(newSingleAnswer);
			}
		}

		private async Task AddPairAnswer(Dictionary<string, string> pairAnswers, Guid testId, Guid questionId)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(questionId, Guid.Empty, guid, string.Empty, testId);
			await _testRepo.AddAnswer(newAnswer);

			foreach (var answer in pairAnswers)
			{
				var pairAnswer = new MatchingPair(guid, answer.Key, answer.Value);
				await _testRepo.AddPairAnswer(pairAnswer);
			}
		}
	}
}
