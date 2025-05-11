namespace ConstructorForTests.Models
{
	public class TestResult
	{
		public Guid Id { get; set; }
		public Guid TestId { get; set; }
		public Guid UserId { get; set; }
		public decimal TotalScore { get; set; }
		public bool IsPassed { get; set; }
	}
}
