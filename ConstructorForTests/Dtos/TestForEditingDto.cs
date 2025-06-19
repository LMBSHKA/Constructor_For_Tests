using ConstructorForTests.Models;

namespace ConstructorForTests.Dtos
{
	public class TestForEditingDto : CreateTestDto
	{
		public List<BaseQuestionDto> Questions { get; set; }

		public TestForEditingDto(Test test, List<BaseQuestionDto> questions)
		{
			Title = test.Title;
			Description = test.Description;
			StartAt = DateTime.ParseExact(test.StartAt, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
			EndAt = DateTime.ParseExact(test.EndAt, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
			ScoreToPass = test.ScoreToPass;
			MessageAboutPassing = test.MessageAboutPassing;
			FailureMessage = test.FailureMessage;
			TimerInSeconds = int.Parse(test.TimerInSeconds!);
			Questions = questions;
			
		}
	}
}
