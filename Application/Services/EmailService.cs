using System.Net.Mail;
using System.Net;
using Domain.Model.UserModel;

public interface IEmailService
{
    Task SendEmailAsync(Email request);
}

public class EmailService : IEmailService
{
    private readonly string fromMail = "ghoranman@gmail.com";
    private readonly string fromPassword = "zukrgrcgsuecienm";

    public async Task SendEmailAsync(Email request)
    {
        try
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.To.Add(new MailAddress(request.To));
            message.Subject = request.Subject;
            message.Body = request.Body;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new Exception("Email send failed: " + ex.Message);
        }
    }
}
