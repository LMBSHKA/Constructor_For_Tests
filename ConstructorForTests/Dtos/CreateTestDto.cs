using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ConstructorForTests.Dtos
{
	public class CreateTestDto
	{
		public string Title { get; set; } = "null";
		public DateTime StartAt { get; set; } = DateTime.MinValue;
		public DateTime EndAt { get; set; } = DateTime.MinValue;
		public bool IsActive { get; set; } = false;
		public decimal ScoreToPass { get; set; } = decimal.Zero;
		public bool ManualCheck { get; set; } = false;
		public List<CreateQuestionDTO>? Questions { get; set; }
	}
}
