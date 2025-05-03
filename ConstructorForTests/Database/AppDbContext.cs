using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Database
{
	public class AppDbContext : DbContext
	{
		public DbSet<Curator> Curators { get; set; }
		public DbSet<Test> Tests { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<MultipleChoice> MultipleChoices { get; set; }
		public DbSet<MatchingPair> MatchingPairs { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	}
}
