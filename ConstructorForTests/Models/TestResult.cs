namespace ConstructorForTests.Models
{
	public class TestResult
	{
		public Guid Id { get; set; }
		public Guid TestId { get; set; }
		public Guid UserId { get; set; }
		public decimal TotalScore { get; set; }
		public bool IsPassed { get; set; }

		public TestResult(Guid testId, Guid userId, decimal totalScore, bool isPassed)
		{
			TestId = testId;
			UserId = userId;
			TotalScore = totalScore;
			IsPassed = isPassed;
		}
	}
}
