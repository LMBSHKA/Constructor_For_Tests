﻿using MimeKit;
using MailKit.Net.Smtp;

namespace ConstructorForTests.API
{
	public class EmailSender : IEmailSender
	{
		public async Task SendEmail(string toAdressEmail, decimal score, string message)
		{
			var fromAdress = "stajirovkiural@gmail.com";
			var title = "Результат теста стажировки";
			var subject = "Результат теста";
			var body = message + $" Ваш балл: {score}";
			using var mimeMessage = new MimeMessage();

			mimeMessage.From.Add(new MailboxAddress(title, fromAdress));
			mimeMessage.To.Add(new MailboxAddress("", toAdressEmail));
			mimeMessage.Subject = subject;
			mimeMessage.Body = new TextPart("Plain")
			{
				Text = body
			};

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync("smtp.gmail.com", 587, false);
				await client.AuthenticateAsync(fromAdress, "bqme vzoy nhkc mbxj");
				await client.SendAsync(mimeMessage);

				await client.DisconnectAsync(true);
			}
		}
	}
}
