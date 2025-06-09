using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class MarkedQuestionDto
	{
		[Required]
		public Guid QuestionId { get; set; }
		[Required]
		public int Mark { get; set; }
	}
}
