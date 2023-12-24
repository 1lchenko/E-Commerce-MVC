namespace ECommerce.Services;

public interface ISendEmailService
{
    bool EmailSend(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false);
}