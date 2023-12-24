using System.Net.Mail;
using System.Net;

namespace ECommerce.Services
{
    public class SendEmailService : ISendEmailService
    {
        private readonly ILogger<SendEmailService> _logger;

        public SendEmailService(ILogger<SendEmailService> logger)
        {
            _logger = logger;
        }
        public bool EmailSend(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false)
        {
            bool status = false;
            try
            {
                string HostAddress = "";
                string FromEmailId = "";
                string Password = " ";
                string Port = " ";
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(FromEmailId);
                mailMessage.Subject = Subject;
                mailMessage.Body = Message;
                mailMessage.IsBodyHtml = IsBodyHtml;
                mailMessage.To.Add(new MailAddress(SenderEmail));
                SmtpClient smtp = new SmtpClient
                {
                    Host = HostAddress,
                    EnableSsl = true
                };
                NetworkCredential networkCredential = new NetworkCredential
                {
                    UserName = mailMessage.From.Address,
                    Password = Password
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCredential;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = Convert.ToInt32(Port);
                smtp.Send(mailMessage);
                status = true;
                _logger.LogInformation("Email will be send {Date} from Host '{HostName}'", DateTime.Now.ToString(), HostAddress);
                return status;
                
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Method {Name} ending with exception. Exception message - '{Message}'. StackTrace - '{StackTrace}'. Source - '{Source}'", 
                    nameof(EmailSend), e.Message, e.StackTrace, e.Source );
                return status;
            }
        }
    }
}
