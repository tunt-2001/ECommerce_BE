using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Hangfire; 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class AdminMarketingService : IAdminMarketingService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AdminMarketingService> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public AdminMarketingService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IApplicationDbContext context,
        ILogger<AdminMarketingService> logger,
        IBackgroundJobClient backgroundJobClient)
    {
        _userManager = userManager;
        _emailService = emailService;
        _context = context;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    /// <summary>
    /// Gửi một bản tin (newsletter) đến tất cả người dùng có email đã được xác thực.
    /// Tác vụ gửi email sẽ được xử lý trong nền bởi Hangfire.
    /// </summary>
    /// <param name="dto">Đối tượng chứa thông tin của bản tin.</param>
    public async Task SendNewsletterAsync(NewsletterDto dto)
    {
        _logger.LogInformation("Starting SendNewsletterAsync process with subject: {Subject}", dto.Subject);

        //Lấy danh sách email của tất cả người dùng hợp lệ
        var userEmails = await _userManager.Users
            .Where(u => u.Email != null && u.EmailConfirmed)
            .Select(u => u.Email!)
            .ToListAsync();

        if (!userEmails.Any())
        {
            _logger.LogWarning("No confirmed users found to send newsletter to. Process aborted.");
            return;
        }

        //Tạo và lưu lại một bản ghi của newsletter vào database
        var newsletter = new Newsletter
        {
            Subject = dto.Subject,
            Body = dto.Body,
            ImageUrl = dto.ImageUrl,
            SentDate = DateTime.UtcNow,
            RecipientCount = userEmails.Count
        };

        try
        {
            _context.Newsletters.Add(newsletter);
            await _context.SaveChangesAsync(default);
            _logger.LogInformation("Newsletter record saved to database with ID: {NewsletterId}", newsletter.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save newsletter record to the database.");
            throw;
        }

        //Sử dụng Hangfire để đưa tác vụ gửi email vào hàng đợi
        try
        {
            _backgroundJobClient.Enqueue<IEmailService>(
                emailService => emailService.SendPromotionalEmailAsync(userEmails, dto.Subject, dto.Body, dto.ImageUrl)
            );

            _logger.LogInformation("Successfully enqueued newsletter for {Count} users. Job will be processed by Hangfire.", userEmails.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while enqueuing the newsletter job with Hangfire.");
            throw;
        }
    }

    /// <summary>
    /// Lấy lịch sử các bản tin đã gửi.
    /// </summary>
    public async Task<List<Newsletter>> GetNewsletterHistoryAsync()
    {
        return await _context.Newsletters
            .OrderByDescending(n => n.SentDate)
            .ToListAsync();
    }
}