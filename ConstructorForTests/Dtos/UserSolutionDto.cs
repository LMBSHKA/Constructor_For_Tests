namespace ConstructorForTests.Dtos
{
	public class UserSolutionDto
	{
		public string? FirstName { get; set; }
		public string? SecondName { get; set; }
		public string? Patronymic { get; set; }
		public string? Email { get; set; }
		public List<UserAnswersDto> Answers { get; set; }
	}
}
