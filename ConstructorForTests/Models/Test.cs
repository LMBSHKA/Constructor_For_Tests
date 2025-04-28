using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class Test
	{
		[Key]
		public Guid Id { get; set; } = Guid.Empty;
		[Required]
		public string Title { get; set; } = "null";
		[Required]
		public string StartAt { get; set; } = String.Empty;
		[Required]
		public string EndAt { get; set; } = String.Empty;
		[Required]
		public bool IsActive { get; set; } = false;
		[Required]
		public decimal ScoreToPass { get; set; } = decimal.Zero;
		[Required]
		public bool ManualCheck { get; set; } = false;

		public Test() { }

		public Test(string title, string startAt, string endAt,
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
