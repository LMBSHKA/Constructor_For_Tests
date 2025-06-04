using ConstructorForTests.API;
using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ConstructorForTests.Repositories
{
	public class TestRepo : ITestRepo
	{
		private readonly AppDbContext _context;
		private readonly ITestHandler _testHandler;
		private int PageSize { get; } = 20;

		public TestRepo(AppDbContext context, ITestHandler testHandler)
		{
			_context = context;
			_testHandler = testHandler;
		}

		public async Task<List<StatisticDto>> GetStatistic(StatisticFilterDto statisticFilter, int pageNumber)
		{
			var statistics = new List<StatisticDto>();
			var listTestResults = GetTestResWithFilter(statisticFilter);

			var pagedItems = await SetPagination(pageNumber, listTestResults);

			foreach (var testResult in pagedItems)
			{
				var user = await _context.Users
					.FirstOrDefaultAsync(x =>
					x.Id == testResult.UserId && 
					EF.Functions.Like(x.Email, $"%{statisticFilter.Email}%"));

				var test = await _context.Tests
					.FirstOrDefaultAsync(x => 
					x.Id == testResult.TestId &&
					EF.Functions.Like(x.Title, $"%{statisticFilter.TestName}%"));

				if (user != null && test != null)
				{
					_testHandler.CreateStatisticDto(statisticFilter, statistics, user, test, testResult);
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

		public async Task<List<Test>> GetAllTests()
		{
			return await _context.Tests.ToListAsync();
		}

		public async Task<Test?> GetTestInfoById(Guid id)
		{
			
			if (id == Guid.Empty)
				return null;

			var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id);

			if (test == null)
				return null;

			return test;
		}

		public async Task<List<Question>> GetTestQuestion(Guid testId)
		{
			var questions = await _context.Questions
				.Where(x => x.TestId == testId)
				.OrderBy(x => x.Order)
				.ToListAsync();

			return questions;
		}

		public async Task<bool> CreateTest(CreateTestDto createTestData, string curatorId)
		{
			try
			{
				if (createTestData.Title == null)
					return false;

				var isActive = false;
				if (createTestData.StartAt <= DateTime.Now)
					isActive = true;

				var newTest = new Test(
					createTestData.Title,
					createTestData.StartAt.ToString("dd.MM.yyyy"),
					createTestData.EndAt.ToString("dd.MM.yyyy"),
					isActive,
					createTestData.ScoreToPass,
					false,
					curatorId!,
					createTestData.MessageAboutPassing!,
					createTestData.FailureMessage!,
					createTestData.TimerInSeconds.ToString()
					);

				await _context.Tests.AddAsync(newTest);
				await _context.SaveChangesAsync();
				await AddQuestion(newTest.Id, createTestData.Questions, newTest);
				await _context.SaveChangesAsync();

				return true;
			}

			catch
			{
				return false;
			}
		}

		private async Task AddQuestion(Guid testId, List<CreateQuestionDTO> questions, Test test)
		{
			var order = 1;
			foreach (var question in questions)
			{
				var newQuestion = new Question(testId, question.QuestionText, question.Type, 1, 
					order, question.AnswerOptions, question.PairKey, question.PairValue);
				await _context.Questions.AddAsync(newQuestion);
				if (question.Type == QuestionType.DetailedAnswer)
				{
					test.ManualCheck = true;
				}

				await AddAnswer(newQuestion.Id, question.CreateAnswer, testId);
				order++;
			}
		}
		
		private async Task AddAnswer(Guid questionId, AnswerDTO answer, Guid testId)
		{
			if (!string.IsNullOrEmpty(answer.TextAnswer))
			{
				var newAnswer = new Answer(questionId, Guid.Empty, Guid.Empty, answer.TextAnswer, testId);
				await _context.Answers.AddAsync(newAnswer);
				var guid = newAnswer.Id;

				var test = await _context.Answers.FirstOrDefaultAsync(x => x.Id == guid);
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
			await _context.Answers.AddAsync(newAnswer);

			foreach (var singleAnswer in multipleAnswers)
			{
				var newSingleAnswer = new MultipleChoice(guid, singleAnswer);
				await _context.MultipleChoices.AddAsync(newSingleAnswer);
			}
		}

		private async Task AddPairAnswer(Dictionary<string, string> pairAnswers, Guid testId, Guid questionId)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(questionId, Guid.Empty, guid, string.Empty, testId);
			await _context.Answers.AddAsync(newAnswer);

			foreach (var answer in pairAnswers)
			{
				var pairAnswer = new MatchingPair(guid, answer.Key, answer.Value);
				await _context.MatchingPairs.AddAsync(pairAnswer);
			}
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
	}
}
