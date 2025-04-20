using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class GetOrCreateTestDto : Test
	{
		[Required]
		public List<Question>? Questions { get; set; }

		public GetOrCreateTestDto() { }
		public GetOrCreateTestDto(Test test, List<Question> questions)
		{
			Title = test.Title;
			StartAt = test.StartAt;
			EndAt = test.EndAt;
			IsActive = test.IsActive;
			ScoreToPass = test.ScoreToPass;
			ManualCheck = test.ManualCheck;
			Questions = questions;
		}
	}
}
