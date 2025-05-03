namespace ConstructorForTests.Dtos
{
	public class CreateAnswerDTO
	{
		public string? TextAnswer { get; set; } = string.Empty;
		public List<string> MultipleAnswer { get; set; } = [];
		public Dictionary<string, string> MatchingPairs { get; set; } = [];
	}
}
