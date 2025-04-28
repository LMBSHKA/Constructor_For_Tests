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

		public async Task<GetOrCreateTestDto?> GetTestById(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id);

			if (test == null)
				return null;

			var questions = await _context.Questions.Where(x => x.TestId == id).ToListAsync();

			return new GetOrCreateTestDto(test, questions);
		}

		public async Task<bool> CreateTest(GetOrCreateTestDto createTestData)
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

		private async Task AddQuestion(Guid testId, List<Question> questions)
		{
			foreach (var question in questions)
			{
				question.TestId = testId;
				await _context.Questions.AddAsync(question);
				await _context.SaveChangesAsync();
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
