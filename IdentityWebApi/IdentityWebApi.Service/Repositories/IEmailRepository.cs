namespace IdentityWebApi.Repositories
{
    public interface IEmailRepository
    {
        Task<bool> SendMail(string email, string subject, string body);
    }
}