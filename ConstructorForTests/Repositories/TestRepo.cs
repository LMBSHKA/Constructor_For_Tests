using ConstructorForTests.API;
using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Repositories
{
	public class TestRepo : ITestRepo
	{
		private readonly AppDbContext _context;
		private readonly ITestHandler _testHandler;
		private readonly IUserRepo _userRepo;
		private readonly IEmailSender _emailSender;
		private int PageSize { get; } = 20;

		public TestRepo(AppDbContext context, ITestHandler testHandler, IUserRepo userRepo, IEmailSender emailSender)
		{
			_emailSender = emailSender;
			_context = context;
			_testHandler = testHandler;
			_userRepo = userRepo;
		}

		public async Task<List<StatisticDto>> GetStatistic(StatisticFilterDto statisticFilter, int pageNumber)
		{
			var statistics = new List<StatisticDto>();
			var listTestResults = GetTestResWithFilter(statisticFilter);

			var pagedItems = await SetPagination(pageNumber, listTestResults);

			foreach (var testResult in pagedItems)
			{
				var test = await _context.Tests
					.FirstOrDefaultAsync(x =>
					x.Id == testResult.TestId &&
					x.IsDelete == false &&
					EF.Functions.Like(x.Title, $"%{statisticFilter.TestName}%"));

				if (test != null)
				{
					var user = await _context.Users
						.FirstOrDefaultAsync(x =>
						x.Id == testResult.UserId &&
						EF.Functions.Like(x.Email, $"%{statisticFilter.Email}%"));

					if (user != null)
					{
						_testHandler.CreateStatisticDto(statisticFilter, statistics, user, test, testResult);
					}
				}
			}

			return statistics;
		}

		private async Task<TestResult[]> SetPagination(int pageNumber, IQueryable<TestResult> listTestResults)
		{
			var startIndex = (pageNumber - 1) * PageSize;
			var pagedItems = await listTestResults
				.Skip(startIndex)
				.Take(PageSize)
				.ToArrayAsync();

			return pagedItems;
		}

		private IQueryable<TestResult> GetTestResWithFilter(StatisticFilterDto statisticFilter)
		{
			var listTestResults = _context.TestResults
				.Where(stat =>
				EF.Functions.Like(stat.IsPassed.ToString(), $"%{statisticFilter.Result}%") &
				EF.Functions.Like(stat.TotalScore.ToString(), $"%{(statisticFilter.Score == -1 ? "" : statisticFilter.Score)}%"));

			return listTestResults;
		}

		public IQueryable<Test> GetAllTests()
		{
			return _context.Tests
				.Where(x => x.IsDelete == false);
		}

		public async Task<Test?> GetTestInfoById(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			var test = await _context.Tests
				.FirstOrDefaultAsync(t => t.Id == id && t.IsDelete == false);

			return test;
		}

		public IOrderedQueryable<Question> GetTestQuestion(Guid testId)
		{
			var questions = _context.Questions
				.Where(x => x.TestId == testId)
				.OrderBy(x => x.Order);

			return questions;
		}

		public async Task CreateTestInfo(Test newTest)
		{
			await _context.Tests.AddAsync(newTest);
		}

		public async Task AddQuestion(Question newQuestion)
		{
			await _context.Questions.AddAsync(newQuestion);
		}

		public async Task AddAnswer(Answer newAnswer)
		{
			await _context.Answers.AddAsync(newAnswer);
		}

		public async Task AddSingleSolutionForMultipleAnswer(MultipleChoice newSingleAnswer)
		{
			await _context.MultipleChoices.AddAsync(newSingleAnswer);
		}

		public async Task AddPairAnswer(MatchingPair pairAnswer)
		{
			await _context.MatchingPairs.AddAsync(pairAnswer);
		}

		public async Task<Answer> GetQuestionsCorrectAnswers(Guid questionId)
		{
			var correctAnswers = await _context.Answers
				.FirstOrDefaultAsync(x => x.QuestionId == questionId);
			return correctAnswers;
		}

		public IQueryable<MultipleChoice> GetMultipleChoice(Guid multipleAnswerId)
		{
			var queryableAnswers = _context.MultipleChoices
				.Where(x => x.MultipleAnswerId == multipleAnswerId);

			return queryableAnswers;
		}

		public IQueryable<MatchingPair> GetMatchingPair(Guid pairId)
		{
			var queryableAnswers = _context.MatchingPairs
				.Where(x => x.PairId == pairId);

			return queryableAnswers;
		}

		public async Task<bool> UpdateTest(Guid id, Test updateTestData)
		{
			if (id != updateTestData.Id)
				return false;

			_context.Entry(updateTestData).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
				return true;
			}

			catch
			{
				return false;
			}
		}

		public async Task<int> DeleteTest(Guid testId)
		{
			var test = await _context.Tests.FirstOrDefaultAsync(x => x.Id == testId);

			if (test != null)
			{
				test.IsDelete = true;
				_context.Update(test);
				await _context.SaveChangesAsync();

				return StatusCodes.Status200OK;
			}

			return StatusCodes.Status404NotFound;
		}

		public async Task<int> UpdateResult(Guid testId, List<ManualCheckDto> manualCheckData)
		{
			var test = await GetTestInfoById(testId);
			if (test == null)
				return StatusCodes.Status404NotFound;

			foreach (var userMarks in manualCheckData)
			{
				var userId = userMarks.UserId;
				var user = await _context.Users
					.FirstOrDefaultAsync(x => x.Id == userId);
				var usersTestResult = await _context.TestResults
					.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);
				if (user == null || usersTestResult == null)
					return StatusCodes.Status404NotFound;

				var markSum = 0;

				foreach (var markedQuestion in userMarks.MarkedQuestions)
				{
					markSum += markedQuestion.Mark;
					await UpdateUserAnswer(userId, markedQuestion.QuestionId);
				}

				usersTestResult.TotalScore += markSum;
				await CheckPassage(test, usersTestResult, user);
				_context.Update(usersTestResult);
			}
			await _context.SaveChangesAsync();

			return StatusCodes.Status200OK;
		}

		public async Task UpdateUserAnswer(Guid userId, Guid questionId)
		{
			var userAnswer = await _context.UserAnswers
				.FirstOrDefaultAsync(x => x.UserId == userId && x.QuestionId == questionId);
			userAnswer.NeedVerification = false;
			_context.Update(userAnswer);

		}

		private async Task CheckPassage(Test test, TestResult usersTestResult, User user)
		{
			if (_userRepo.CheckPassage(test, usersTestResult.TotalScore))
			{
				usersTestResult.IsPassed = true;
				await _emailSender.SendEmail(user.Email!, usersTestResult.TotalScore, test.MessageAboutPassing);
			}
			else
				await _emailSender.SendEmail(user.Email!, usersTestResult.TotalScore, test.FailureMessage);
		}

		public async Task<List<SendTestToCheckDto>> CreateTestDtoToCheck(Guid testId)
		{
			var questions = await _context.Questions
				.Where(x => x.TestId == testId && x.Type == QuestionType.DetailedAnswer)
				.ToListAsync();
			var listTestToSend = new List<SendTestToCheckDto>();

			foreach (var question in questions)
			{
				var userAnswers = _context.UserAnswers
					.Where(x => x.QuestionId == question.Id && x.NeedVerification == true);
				_testHandler.CreateListToSend(listTestToSend, userAnswers, question);
			}

			return listTestToSend;
		}
		public async Task SaveChecnhesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
