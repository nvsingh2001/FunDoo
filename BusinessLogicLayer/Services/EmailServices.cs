using System.Net;
using System.Net.Mail;
using BusinessLogicLayer.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayer.Services;

public class EmailServices(IConfiguration configuration): IEmailService
{
    private const string SenderEmail = "no-reply@funDoo.com";
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var client = new SmtpClient(configuration["SmtpSettings:Host"], Convert.ToInt32(configuration["SmtpSettings:Port"]))
        {
            Credentials = new NetworkCredential(configuration["SmtpSettings:Username"], configuration["SmtpSettings:Password"] ),
            EnableSsl = true
        }; 
        
        var mail = new MailMessage(
            SenderEmail,
            toEmail)
        {
            Subject = subject,
            Body = message
        };
        
        await client.SendMailAsync(mail);
    }
}