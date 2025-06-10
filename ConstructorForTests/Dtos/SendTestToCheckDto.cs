namespace ConstructorForTests.Dtos
{
	public class SendTestToCheckDto
	{
		public Guid Userid { get; set; }
		public List<QuestionToCheckDto> QuestionToCheckDtos { get; set; }

		public SendTestToCheckDto(Guid userid, List<QuestionToCheckDto> questionToCheckDtos)
		{
			Userid = userid;
			QuestionToCheckDtos = questionToCheckDtos;
		}
	}
}
