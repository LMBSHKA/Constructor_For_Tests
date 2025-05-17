namespace ConstructorForTests.API
{
	public interface IEmailSender
	{
		Task SendEmail(string toAdressEmail, decimal score, bool isPassed);
	}
}
