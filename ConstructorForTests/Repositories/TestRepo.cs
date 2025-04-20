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

		public async Task<Test?> GetTestById(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			return await _context.Tests.FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<bool> CreateTest(Test createTestData)
		{
			try
			{
				if (createTestData.Title == "null")
					return false;

				createTestData.IsActive = true;
				createTestData.ManualCheck = true;

				await _context.AddAsync(createTestData);
				await _context.SaveChangesAsync();

				return true;
			}

			catch
			{
				return false;
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
