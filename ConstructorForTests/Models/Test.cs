using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class Test
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public string? Title { get; set; }
		[Required]
		public DateTime StartAt { get; set; }
		[Required]
		public DateTime EndAt { get; set; }
		[Required]
		public bool IsActive {  get; set; }
		[Required]
		public decimal ScoreToPass { get; set; }
		[Required]
		public bool ManualCheck { get; set; }

		public Test() { }

		public Test(string title, DateTime startAt, DateTime endAt,
			bool isActive, decimal scoreToPass, bool manualCheck)
		{
			Title = title;
			StartAt = startAt;
			EndAt = endAt;
			IsActive = isActive;
			ScoreToPass = scoreToPass;
			ManualCheck = manualCheck;
		}
	}
}
