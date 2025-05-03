using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class MatchingPair
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid PairId { get; set; }
		[Required]
		public string? PairKey { get; set; }
		[Required]
		public string? PairValue { get; set; }

		public MatchingPair() { }

		public MatchingPair(Guid pairId, string? pairKey, string? pairValue)
		{
			PairId = pairId;
			PairKey = pairKey;
			PairValue = pairValue;
		}
	}
}
