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
		private readonly IEmailSender _sender;
		private readonly ITestHandler _testHandler;
		public TestRepo(AppDbContext context, IEmailSender sender, ITestHandler testHandler)
		{
			_sender = sender;
			_context = context;
			_testHandler = testHandler;
		}

		public async Task<List<StatisticDto>> GetStatistic()
		{
			var statistics = new List<StatisticDto>();
			var listTestResults = await _context.TestResults.ToListAsync();
			
			foreach (var testResult in listTestResults)
			{
				var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == testResult.UserId);
				var test = await _context.Tests.FirstOrDefaultAsync(x => x.Id == testResult.TestId);
				if (user != null && test != null)
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

			return statistics;
		}

		public async Task<List<Test>> GetAllTests()
		{
			return await _context.Tests.ToListAsync();
		}

		public async Task<GetTestDTO?> GetTestById(Guid id, ISession session)
		{
			
			if (id == Guid.Empty)
				return null;

			var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id);

			if (test == null)
				return null;

			if (test.IsActive == false && session.GetString("CuratorId") == null)
				return null;

			var questions = await _context.Questions
				.Where(x => x.TestId == id)
				.OrderBy(x => x.Order)
				.ToListAsync();

			var listGetQuestions = new List<BaseQuestionDto>();

			_testHandler.GetTestById(listGetQuestions, questions);

			return new GetTestDTO(test, listGetQuestions);
		}

		public async Task<bool> CreateTest(CreateTestDto createTestData, ISession session)
		{
			try
			{
				if (createTestData.Title == "null")
					return false;

				var newTest = new Test(
					createTestData.Title,
					createTestData.StartAt.ToString("dd.MM.yyyy"),
					createTestData.EndAt.ToString("dd.MM.yyyy"),
					true,
					createTestData.ScoreToPass,
					false,
					session.GetString("CuratorId")!
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
