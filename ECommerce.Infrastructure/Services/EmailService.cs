using ECommerce.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_config["MailSettings:SenderName"], _config["MailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync(_config["MailSettings:SmtpHost"], int.Parse(_config["MailSettings:SmtpPort"]!), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["MailSettings:SmtpUser"], _config["MailSettings:SmtpPass"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}