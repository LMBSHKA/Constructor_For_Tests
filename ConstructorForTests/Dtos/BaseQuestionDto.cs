using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConstructorForTests.Dtos
{
	[JsonDerivedType(typeof(QuestionWithOptionsDto))]
	[JsonDerivedType(typeof(QuestionWithPairDto))]
	[JsonDerivedType(typeof(SimpleQuestionDto))]
	public class BaseQuestionDto
	{
		public Guid Id { get; set; } = Guid.Empty;
		public Guid TestId { get; set; } = Guid.Empty;
		public string? QuestionText { get; set; }
		public QuestionType Type { get; set; }
		public string[]? Test { get; set; } = null;
	}
}
