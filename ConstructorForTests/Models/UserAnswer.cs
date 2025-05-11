namespace ConstructorForTests.Models
{
	public class UserAnswer
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid QuestionId { get; set; }
		public Guid MultipleAnswerId { get; set; }
		public Guid PairId { get; set; }
		public string? Text { get; set; }
	}
}
