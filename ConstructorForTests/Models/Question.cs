using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class Question
	{
		[Key]
		public Guid Id { get; set; } = Guid.Empty;
		public Guid TestId { get; set; } = Guid.Empty;
		[Required]
		public string? QuestionText { get; set; }
		[Required]
		public QuestionType Type { get; set; }
		[Required]
		public decimal Mark { get; set; }
		[Required]
		public int Order { get; set; }
	}

	public enum QuestionType
	{
		SingleAnswer = 1,
		MultiplyAnswer,
		MatchingPair,
		DetailedAnswer
	}
}
