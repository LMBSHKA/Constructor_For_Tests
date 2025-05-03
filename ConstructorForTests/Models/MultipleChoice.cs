using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class MultipleChoice
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid MultipleAnswerId { get; set; }
		[Required]
		public string? Answer { get; set; }

		public MultipleChoice() { }

		public MultipleChoice(Guid multipleAnswerId, string answer)
		{
			MultipleAnswerId = multipleAnswerId;
			Answer = answer;
		}
	}
}
