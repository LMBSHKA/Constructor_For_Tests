using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class UserMultipleChoice
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid MultipleAnswerId { get; set; }
		[Required]
		public string? Answer { get; set; }
	}
}
