﻿using ConstructorForTests.Models;
using System.Globalization;

namespace ConstructorForTests.Dtos
{
	public class SendTestDTO : CreateTestDto
	{
		public List<BaseQuestionDto> Questions { get; set; }

		public SendTestDTO() { }
		public bool IsActive { get; set; }
		public bool ManualCheck { get; set; }

		public SendTestDTO(Test test, List<BaseQuestionDto> questions)
		{
			Title = test.Title;
			Description = test.Description;
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
