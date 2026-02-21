namespace ASP_NET_Final_Proj.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; } = string.Empty;
    public string Email { get; set; }
    public string? PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = null!;
    public DateTimeOffset? DeletedAt { get; set; } = null;
    public ICollection<Invoice> Invoices { get; set; }
}
