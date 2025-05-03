using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class Answer
	{
		[Key]
		public Guid Id { get; set; }
		public Guid QuestionId { get; set; }
		public Guid MultipleAnswerId { get; set; }
		public Guid PairId { get; set; }
		public string TextAnswer { get; set; }

		public Answer() { }

		public Answer (Guid questionId, Guid multipleAnswerId, Guid pairId, string textAnswer)
		{
			QuestionId = questionId;
			MultipleAnswerId = multipleAnswerId;
			PairId = pairId;
			TextAnswer = textAnswer;
		}
	}
}
