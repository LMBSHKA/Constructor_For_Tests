using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ConstructorForTests.Dtos
{
	public class GetOrCreateTestDto
	{
		public string Title { get; set; } = "null";
		public DateTime StartAt { get; set; } = DateTime.MinValue;
		public DateTime EndAt { get; set; } = DateTime.MinValue;
		public bool IsActive { get; set; } = false;
		public decimal ScoreToPass { get; set; } = decimal.Zero;
		public bool ManualCheck { get; set; } = false;
		public List<Question>? Questions { get; set; }

		public GetOrCreateTestDto() { }
		public GetOrCreateTestDto(Test test, List<Question> questions)
		{
			Title = test.Title;
			StartAt = DateTime.ParseExact(test.StartAt, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			EndAt = DateTime.ParseExact(test.EndAt, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			IsActive = test.IsActive;
			ScoreToPass = test.ScoreToPass;
			ManualCheck = test.ManualCheck;
			Questions = questions;
		}
	}
}
