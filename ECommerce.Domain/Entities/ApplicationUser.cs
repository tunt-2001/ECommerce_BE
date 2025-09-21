using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
}
        