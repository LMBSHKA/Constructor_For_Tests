using ConstructorForTests.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class Test
	{
		[Key]
		public Guid Id { get; set; } = Guid.Empty;
		[Required]
		public string UserId { get; set; } = null;
		[Required]
		public string Title { get; set; } = null;
		[Required]
		public string Description { get; set; } = null;
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
		[Required]
		public string MessageAboutPassing { get; set; } = null;
		[Required]
		public string FailureMessage { get; set; } = null;
		public string? TimerInSeconds { get; set; } = null;
		public bool IsDelete { get; set; } = false;

		public Test() { }

		public Test(CreateTestDto createTest, bool isAvtive, bool manualCheck, string curatorId)
		{
			UserId = curatorId;
			Title = createTest.Title!;
			Description = createTest.Description!;
			StartAt = createTest.StartAt.ToString("dd.MM.yyyy");
			EndAt = createTest.EndAt.ToString("dd.MM.yyyy");
			IsActive = isAvtive;
			ScoreToPass = createTest.ScoreToPass;
			ManualCheck = manualCheck;
			MessageAboutPassing = createTest.MessageAboutPassing!;
			FailureMessage = createTest.FailureMessage!;
			TimerInSeconds = createTest.TimerInSeconds.ToString();
		}
	}
}
