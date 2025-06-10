namespace ConstructorForTests.Dtos
{
	public class SendTestToCheckDto
	{
		public Guid Userid { get; set; }
		public List<QuestionToCheckDto> QuestionToCheckDtos { get; set; }
	}
}
