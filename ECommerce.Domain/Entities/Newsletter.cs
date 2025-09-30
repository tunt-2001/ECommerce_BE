using System;

namespace ECommerce.Domain.Entities;

public class Newsletter
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
    public int RecipientCount { get; set; } // Số người đã nhận
}