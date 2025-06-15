using ConstructorForTests.Dtos;
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
		public string? AnswerOptions { get; set; } = string.Empty;
		public string? PairKey { get; set; } = string.Empty;
		public string? PairValue { get; set;} = string.Empty;
		[Required]
		public QuestionType Type { get; set; }
		[Required]
		public decimal Mark { get; set; }
		[Required]
		public int Order { get; set; }

		public Question() { }

		public Question(Guid testId, CreateQuestionDTO newQuestion, int order, decimal mark)
		{
			TestId = testId;
			QuestionText = newQuestion.QuestionText;
			AnswerOptions = newQuestion.AnswerOptions;
			PairKey = newQuestion.PairKey;
			PairValue = newQuestion.PairValue;
			Type = newQuestion.Type;
			Mark = mark;
			Order = order;
		}
	}

	public enum QuestionType
	{
		Null = 0,
		SingleAnswer,
		MultiplyAnswer,
		MatchingPair,
		DetailedAnswer
	}
}
