using TQM.Backoffice.Application.DTOs.Mail;


namespace TQM.Backoffice.Application.Infrastructure.Persistence.Contracts;

public interface IMailServiceX
{
    Task SendEmailAsync(MailDTOs.MailRequest mailRequest);
    //Task SendMail(MailDTOs.MailRequest mailRequest);
}
