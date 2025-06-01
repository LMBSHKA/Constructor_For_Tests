using ConstructorForTests.Models;
using System.Globalization;

namespace ConstructorForTests.Dtos
{
	public class GetTestDTO : CreateTestDto
	{
		public List<BaseQuestionDto> Questions { get; set; }

		public GetTestDTO() { }
		public bool ManualCheck { get; set; }

		public GetTestDTO(Test test, List<BaseQuestionDto> questions)
		{
			Title = test.Title;
			StartAt = DateTime.ParseExact(test.StartAt, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			EndAt = DateTime.ParseExact(test.EndAt, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			IsActive = test.IsActive;
			ScoreToPass = test.ScoreToPass;
			ManualCheck = test.ManualCheck;
			Questions = questions;
			TimerInSeconds = int.Parse(test.TimerInSeconds!);
		}
	}
}
