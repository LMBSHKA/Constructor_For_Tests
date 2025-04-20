namespace ConstructorForTests.Database
{
	public class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
			}
		}

		public static void SeedData(AppDbContext context)
		{
			if (!context.Curators.Any())
			{
				context.Curators.AddRange(
					new Models.Curator
					{
						Id = new Guid(),
						Email = "Admin@gmail.com",
						Password = "1234"
					});

				context.SaveChanges();
			}

			if (!context.Tests.Any())
			{
				context.Tests.AddRange(
					new Models.Test("test", DateTime.Now, DateTime.Now, true, 10, false),
					new Models.Test("test2", DateTime.Now, DateTime.Now, false, 30, true)
					);

				context.SaveChanges();
			}
		}
	}
}
