using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Database
{
	public class AppDbContext : DbContext
	{
		public DbSet<Curator> Curators { get; set; }
		public DbSet<Test> Tests { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	}
}
