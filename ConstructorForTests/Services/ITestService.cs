using ConstructorForTests.Dtos;

namespace ConstructorForTests.Services
{
	public interface ITestService
	{
		Task<SendTestDTO?> GetTest(Guid testId, bool isCurator);
		string CalculateTimer(int timerInSeconds, string startTimer);
	}
}
