using ConstructorForTests.Models;
using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class CreateQuestionDTO
	{
		public string? QuestionText { get; set; }
		public QuestionType Type { get; set; }
		public AnswerDTO CreateAnswer { get; set; }
	}
}
