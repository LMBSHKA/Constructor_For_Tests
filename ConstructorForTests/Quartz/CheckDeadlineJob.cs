using ConstructorForTests.Repositories;
using Quartz;
using System.Globalization;

namespace ConstructorForTests.Quartz
{
	public class CheckDeadlineJob : IJob
	{
		private readonly ITestRepo _testRepo;

		public CheckDeadlineJob(ITestRepo testRepo)
		{
			_testRepo = testRepo;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var allTests = await _testRepo.GetAllTests();

			foreach (var test in allTests)
			{
				if (DateTime.ParseExact(test.StartAt, "d", new CultureInfo("ru-RU")) >= DateTime.Now)
				{
					test.IsActive = true;
					await _testRepo.UpdateTest(test.Id, test);
				}
				if (DateTime.ParseExact(test.EndAt, "d", new CultureInfo("ru-RU")) <= DateTime.Now)
				{
					test.IsActive = false;
					await _testRepo.UpdateTest(test.Id, test);
				}
			}

			return;
		}
	}
}
