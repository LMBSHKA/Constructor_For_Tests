using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class CreateQuestionDTO
	{
		public string? QuestionText { get; set; }
		public string? AnswerOptions { get; set; } = string.Empty;
		public string? PairKey { get; set; } = string.Empty;
		public string? PairValue { get; set; } = string.Empty;
		public QuestionType Type { get; set; }
		public decimal Mark { get; set; }
		public AnswerDTO CreateAnswer { get; set; }
	}
}
