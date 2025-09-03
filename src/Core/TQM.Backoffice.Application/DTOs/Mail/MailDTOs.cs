namespace TQM.Backoffice.Application.DTOs.Mail
{
    public class MailDTOs
    {
        public class MailSettings
        {
            public string Mail { get; set; } = String.Empty;
            public string DisplayName { get; set; } = String.Empty;
            //public string Password { get; set; } = String.Empty;
            public string Host { get; set; } = String.Empty;
            public int Port { get; set; } = 0;

            public string MailDisplay 
            {
                get {
                    return $" \"{this.DisplayName}\" <{this.Mail}>";
                }
                set {}
            }
            
        }

        public class MailRequest
        {
            public List<string> ToEmail { get; set; } = new();
            public List<string> CCEmail { get; set; } = new();
            public List<string> BCCEmail { get; set; } = new();
            public string Subject { get; set; } = String.Empty;
            public string Body { get; set; } = String.Empty;
            public List<Microsoft.AspNetCore.Http.IFormFile> Attachments { get; set; } = new();
        }

        public class ForgetPasswordMailRequest 
        {
            public string UserMail { get; set; } = String.Empty;
        }

        public class SendUserPasswordMailRequest 
        {
            public string UserMail { get; set; } = String.Empty;
            public string Password { get; set; } = String.Empty;
        }
    }
}