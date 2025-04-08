
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Carproject.Services
{
    public class EmailService
    {
        //ایمیل سرویس قبلی
        //private readonly IConfiguration _configuration;

        //public EmailService(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //public async Task SendEmailAsync(string to, string subject, string body)
        //{
        //    using (var client = new SmtpClient("smtp.example.com"))
        //    {
        //        client.Credentials = new NetworkCredential("your-email@example.com", "your-password");
        //        client.EnableSsl = true;

        //        var mailMessage = new MailMessage
        //        {
        //            From = new MailAddress("your-email@example.com"),
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };

        //        mailMessage.To.Add(to);
        //        await client.SendMailAsync(mailMessage);
        //    }
        //}
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();

            // تعریف فرستنده و گیرنده
            message.From.Add(new MailboxAddress("Your Name", "fatemehabolihosna@gmail.com"));
            message.To.Add(new MailboxAddress("Recipient Name", toEmail));

            // موضوع و محتوا
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // استفاده از SmtpClient برای ارسال ایمیل
            using (var client = new SmtpClient())
            {
                // اتصال به سرور SMTP Gmail
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // اگر احراز هویت دو مرحله‌ای فعال نیست، از پسورد معمولی استفاده کنید
                // در غیر این صورت باید رمز عبور مخصوص برنامه وارد کنید
                await client.AuthenticateAsync("fatemehabolihosna@gmail.com", "13871387f");

                // ارسال ایمیل
                await client.SendAsync(message);

                // قطع ارتباط از سرور
                await client.DisconnectAsync(true);
            }
        }
    }
}
