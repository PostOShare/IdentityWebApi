using IdentityWebApiCommon.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityWebApi.Repositories
{
    public class EmailRepository: IEmailRepository
    {
        private readonly MailSettings _mailSettings;

        public EmailRepository(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        /// <summary>
        /// Sends a mail to the user's email
        /// </summary>
        /// <returns>A Boolean whether the email was sent</returns>
        public async Task<bool> SendMail(string email, string subject, string body)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    var emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmailRepository);
                    emailMessage.From.Add(emailFrom);
                    var emailTo = new MailboxAddress("", email);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = subject;

                    var emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.TextBody = body;

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using (SmtpClient mailClient = new SmtpClient())
                    {
                        await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, 
                                                      MailKit.Security.SecureSocketOptions.StartTls);
                        await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
