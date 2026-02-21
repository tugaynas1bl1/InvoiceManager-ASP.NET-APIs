using Microsoft.AspNetCore.Identity;

namespace ASP_NET_Final_Proj.Models;

public class User : IdentityUser
{
    public string Name { get; set; }
    public string? Address { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int CodeVerification { get; set; } = default;
    public DateTimeOffset? CodeVerificationExpiresAt { get; set; }
    public ICollection<Customer> Customers { get; set; } =
        new List<Customer>();
}
