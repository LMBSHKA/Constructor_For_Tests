namespace ConstructorForTests.Dtos
{
	public class QuestionToCheckDto
	{
		public Guid QuestionId { get; set; }
		public string? QuestionText { get; set; }
		public string? UserAnswer { get; set; }

		public QuestionToCheckDto(Guid questionId, string? questionText, string? userAnswer)
		{
			QuestionId = questionId;
			QuestionText = questionText;
			UserAnswer = userAnswer;
		}
	}
}
