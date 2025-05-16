using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class UserMatchingPair
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid PairId { get; set; }
		[Required]
		public string? PairKey { get; set; }
		[Required]
		public string? PairValue { get; set; }
	}
}
