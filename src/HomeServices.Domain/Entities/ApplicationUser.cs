using Microsoft.AspNetCore.Identity;

namespace HomeServices.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public ProviderProfile? ProviderProfile { get; set; }
    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public ICollection<ServiceRequest> CustomerRequests { get; set; } = new List<ServiceRequest>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
