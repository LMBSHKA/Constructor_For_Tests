using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ConstructorForTests.Dtos
{
	public class CreateTestDto
	{
		public string? Title { get; set; } = null;
		public DateTime StartAt { get; set; } = DateTime.MinValue;
		public DateTime EndAt { get; set; } = DateTime.MinValue;
		public decimal ScoreToPass { get; set; } = decimal.Zero;
		public string? MessageAboutPassing { get; set; } = null;
		public string? FailureMessage { get; set; } = null;
		public int TimerInSeconds { get; set; }
		public List<CreateQuestionDTO>? Questions { get; set; }
	}
}
