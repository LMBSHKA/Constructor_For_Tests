using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class ManualCheckDto
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public List<MarkedQuestionDto> MarkedQuestions { get; set; }
	}
}
