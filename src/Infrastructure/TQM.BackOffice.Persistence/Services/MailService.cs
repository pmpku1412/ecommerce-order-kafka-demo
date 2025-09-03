using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TQM.Backoffice.Application.DTOs.Mail;
using TQM.Backoffice.Domain.ValueObjects;
using MimeKit;
using MailKit;
using Dapper;
using static TQM.Backoffice.Application.DTOs.Mail.MailDTOs;

namespace TQM.BackOffice.Persistence.Services
{
    public class MailService : IMailServiceX
    {
        private readonly IDBAdapter _dbAdapter;

        //public MailService(IDBAdapter dbAdapter) => _dbAdapter = dbAdapter;

        private readonly MailDTOs.MailSettings _mailSettings;
        public MailService(IOptions<MailDTOs.MailSettings> mailSettings, IDBAdapter dbAdapter)//,IWebHostEnvironment _environment)
        {
            _mailSettings = mailSettings.Value;
            _dbAdapter = dbAdapter;
            //Environment = _environment;
        }
        //public MailService(Mail.MailSettings mailSettings) => _mailSettings = mailSettings;

        //private IWebHostEnvironment Environment;

        public async Task SendEmailAsync(MailDTOs.MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.MailDisplay);
            email.From.Add(MailboxAddress.Parse(_mailSettings.MailDisplay));
            //email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            foreach (var itemTo in mailRequest.ToEmail)
            {
                email.To.Add(MailboxAddress.Parse(itemTo));
            }

            foreach (var itemCC in mailRequest.CCEmail)
            {
                email.Cc.Add(MailboxAddress.Parse(itemCC));
            }

            foreach (var itemBCC in mailRequest.BCCEmail)
            {
                email.Bcc.Add(MailboxAddress.Parse(itemBCC));
            }

            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            // using var smtp = new MailKit.Net.Smtp.SmtpClient();
            // smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            // //smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            // await smtp.SendAsync(email);
            // smtp.Disconnect(true);

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.None).ConfigureAwait(false);
                //await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password); // Comment for now ; Test no Password
                await smtp.SendAsync(email).ConfigureAwait(false);
                await smtp.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}