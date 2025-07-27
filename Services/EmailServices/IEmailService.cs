namespace LinkShorterAPI.Services.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
