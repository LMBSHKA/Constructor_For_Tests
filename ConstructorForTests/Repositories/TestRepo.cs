using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

		public async Task<GetTestDTO?> GetTestById(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id);

			if (test == null)
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
				await AddQuestion(testId, createTestData.Questions);

				return true;
			}

			catch
			{
				return false;
			}
		}

		private async Task AddQuestion(Guid testId, List<CreateQuestionDTO> questions)
		{
			foreach (var question in questions)
			{
				var newQuestion = new Question(testId, question.QuestionText, question.Type, question.Mark, question.Order);
				await _context.Questions.AddAsync(newQuestion);
				var questionId = newQuestion.Id;
				await _context.SaveChangesAsync();

				await AddAnswer(questionId, question.CreateAnswer);
			}
		}
		
		private async Task AddAnswer(Guid questionId, CreateAnswerDTO answer)
		{
			if (!string.IsNullOrEmpty(answer.TextAnswer)) 
			{
				var newAnswer = new Answer(questionId, Guid.Empty, Guid.Empty, answer.TextAnswer);
				await _context.Answers.AddAsync(newAnswer);
				var guid = newAnswer.Id;
				await _context.SaveChangesAsync();

				var test = _context.Answers.FirstOrDefaultAsync(x => x.Id == guid);
			}


			if (answer.MultipleAnswer.Count > 0)
			{
				await AddMultipleAnswer(answer.MultipleAnswer);
			}

			if (answer.MatchingPairs.Count > 0)
			{
				await AddPairAnswer(answer.MatchingPairs);
			}
		}

		private async Task AddMultipleAnswer(List<string> multipleAnswers)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(Guid.Empty, guid, Guid.Empty, string.Empty);
			await _context.Answers.AddAsync(newAnswer);
			await _context.SaveChangesAsync();

			foreach (var singleAnswer in multipleAnswers)
			{
				var newSingleAnswer = new MultipleChoice(guid, singleAnswer);
				await _context.MultipleChoices.AddAsync(newSingleAnswer);
				
			}
			await _context.SaveChangesAsync();
		}

		private async Task AddPairAnswer(Dictionary<string, string> pairAnswers)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(Guid.Empty, Guid.Empty, guid, string.Empty);
			await _context.Answers.AddAsync(newAnswer);
			await _context.SaveChangesAsync();

			foreach (var answer in pairAnswers)
			{
				var pairAnswer = new MatchingPair(guid, answer.Key, answer.Value);
				await _context.MatchingPairs.AddAsync(pairAnswer);
			}
			await _context.SaveChangesAsync();
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
