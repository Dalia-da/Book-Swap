using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BookSwap.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string smtpServer = emailSettings["SmtpServer"];
            int port = int.Parse(emailSettings["SmtpPort"]);
            string senderName = emailSettings["SenderName"];
            string senderEmail = emailSettings["SenderEmail"];
            string senderPassword = emailSettings["SenderPassword"];

            var message = new MailMessage();
            message.From = new MailAddress(senderEmail, senderName);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}
