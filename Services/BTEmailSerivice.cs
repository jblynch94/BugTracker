using BugTracker.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BugTracker.Services
{
    public class BTEmailSerivice : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        public BTEmailSerivice(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");

            //add all email addresses to the "TO" for the email
            MimeMessage newEmail = new();
            newEmail.Sender = MailboxAddress.Parse(email);


            newEmail.To.Add(MailboxAddress.Parse(emailSender));

            //add the subject for the email
            newEmail.Subject = subject;

            //add the body for the email
            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //send the email
            using MailKit.Net.Smtp.SmtpClient smtpClient = new();
            try
            {
                var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword"));

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch
            {
                throw;
            }
        }
    }
}
