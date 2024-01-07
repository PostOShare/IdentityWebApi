namespace IdentityWebApi.Services
{
    public interface IEmailService
    {
        Task<bool> SendMail(string email, string subject, string body);
    }
}
