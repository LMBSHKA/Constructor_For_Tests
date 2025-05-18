namespace ConstructorForTests.Dtos
{
	public class StatisticDto
	{
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? TestName { get; set; }
		public bool Result { get; set; }
		public decimal Score { get; set; }

		public StatisticDto(string? fullName, string? email, string? testName, bool result, decimal score)
		{
			FullName = fullName;
			Email = email;
			TestName = testName;
			Result = result;
			Score = score;
		}
	}
}
