using ECommerce.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services; // Thêm namespace để nhất quán

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<EmailService> _logger;
    public EmailService(IConfiguration config, IWebHostEnvironment env, ILogger<EmailService> logger)
    {
        _config = config;
        _env = env;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_config["MailSettings:SenderName"], _config["MailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["MailSettings:SmtpHost"], int.Parse(_config["MailSettings:SmtpPort"]!), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["MailSettings:SmtpUser"], _config["MailSettings:SmtpPass"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendPromotionalEmailAsync(List<string> toEmails, string subject, string body, string? imageUrl)
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", "PromotionTemplate.html");
        if (!File.Exists(templatePath))
        {
            _logger.LogError("Email template not found at path: {Path}", templatePath);
            return;
        }
        var template = await File.ReadAllTextAsync(templatePath);

        template = template.Replace("{{Subject}}", subject);
        template = template.Replace("{{Body}}", body);
        template = template.Replace("{{StoreName}}", _config["MailSettings:SenderName"]);

        // === SỬA LẠI LOGIC TẠO LINK Ở ĐÂY ===
        // Lấy URL của frontend. Nếu không có, mặc định là localhost:3000
        var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:3000";

        if (!string.IsNullOrEmpty(imageUrl))
        {
            // imageUrl nhận được đã là URL công khai. Nút bấm và link ảnh sẽ trỏ đến FrontendUrl.
            var imageHtml = $"<a href='{frontendUrl}' target='_blank'><img src='{imageUrl}' alt='Promotional Image' class='product-image'></a>" +
                            $"<p style='text-align:center; margin-top:20px;'><a href='{frontendUrl}' target='_blank' class='cta-button'>Shop Now</a></p>";
            template = template.Replace("{{ImageSection}}", imageHtml);
        }
        else
        {
            // Nếu không có ảnh, chỉ hiển thị nút "Shop Now"
            var buttonHtml = $"<p style='text-align:center; margin-top:20px;'><a href='{frontendUrl}' target='_blank' class='cta-button'>Shop Now</a></p>";
            template = template.Replace("{{ImageSection}}", buttonHtml);
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_config["MailSettings:SenderName"], _config["MailSettings:SenderEmail"]));
        foreach (var toEmail in toEmails)
        {
            email.To.Add(MailboxAddress.Parse(toEmail));
        }
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = template };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["MailSettings:SmtpHost"], int.Parse(_config["MailSettings:SmtpPort"]!), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["MailSettings:SmtpUser"], _config["MailSettings:SmtpPass"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("Promotional email sent to {Count} recipients.", toEmails.Count);
    }
}