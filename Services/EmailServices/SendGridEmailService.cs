using SendGrid;
using SendGrid.Helpers.Mail;

namespace LinkShorterAPI.Services.EmailServices
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("PUT-YOUR-COMPANY-WORK-EMAIL-HERE", "SHORT LINK");
            var to = new EmailAddress(toEmail);
            var plainTextContent = message;
            var htmlContent = $"<strong>{message}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }

}
