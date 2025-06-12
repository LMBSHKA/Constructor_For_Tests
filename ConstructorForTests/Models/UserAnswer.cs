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
		public bool NeedVerification { get; set; } = false;

		public UserAnswer(Guid userId, Guid questionId, Guid multipleAnswerId, 
			Guid pairId, string text, bool needVerification)
		{
			UserId = userId;
			QuestionId = questionId;
			MultipleAnswerId = multipleAnswerId;
			PairId = pairId;
			Text = text;
			NeedVerification = needVerification;
		}
	}
}
