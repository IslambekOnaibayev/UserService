namespace Core.Interfaces
{
    public interface IEmailSender
    {
        Task SendConfirmationEmailAsync(string toEmail, Guid userId, string token, CancellationToken cancellationToken = default);
    }
}
