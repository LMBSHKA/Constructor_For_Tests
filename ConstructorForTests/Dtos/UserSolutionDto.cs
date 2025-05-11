using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class UserSolutionDto
	{
		[Required]
		public Guid TestId { get; set; }
		public List<UserAnswersDto> Answers { get; set; }
	}
}
