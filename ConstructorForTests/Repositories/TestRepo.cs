using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Repositories
{
	public class TestRepo : ITestRepo
	{
		private readonly AppDbContext _context;
		public TestRepo(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Test>> GetAllTests()
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
				.ToListAsync();

			return new GetTestDTO(test, questions);
		}

		public async Task<bool> CreateTest(CreateTestDto createTestData)
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
					false
					);

				await _context.Tests.AddAsync(newTest);
				var testId = newTest.Id;
				await _context.SaveChangesAsync();
				await AddQuestion(testId, createTestData.Questions, newTest);

				return true;
			}

			catch
			{
				return false;
			}
		}

		private async Task AddQuestion(Guid testId, List<CreateQuestionDTO> questions, Test test)
		{
			foreach (var question in questions)
			{
				var newQuestion = new Question(testId, question.QuestionText, question.Type, question.Mark, question.Order);
				if (question.Type == QuestionType.DetailedAnswer)
				{
					test.ManualCheck = true;
					_context.Tests.Update(test);
					await _context.SaveChangesAsync();
				}

				await _context.Questions.AddAsync(newQuestion);
				var questionId = newQuestion.Id;

				await AddAnswer(questionId, question.CreateAnswer, testId);
				await _context.SaveChangesAsync();
			}
		}
		
		private async Task AddAnswer(Guid questionId, AnswerDTO answer, Guid testId)
		{
			if (!string.IsNullOrEmpty(answer.TextAnswer)) 
			{
				var newAnswer = new Answer(questionId, Guid.Empty, Guid.Empty, answer.TextAnswer, testId);
				await _context.Answers.AddAsync(newAnswer);
				var guid = newAnswer.Id;

				var test = _context.Answers.FirstOrDefaultAsync(x => x.Id == guid);
			}


			if (answer.MultipleAnswer.Count > 0)
			{
				await AddMultipleAnswer(answer.MultipleAnswer, testId, questionId);
			}

			if (answer.MatchingPairs.Count > 0)
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
