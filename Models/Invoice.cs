namespace ASP_NET_Final_Proj.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; } // Foreign key
    public Customer Customer { get; set; } // Navigation prop
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public ICollection<InvoiceRow> Rows { get; set; }
    public decimal TotalSum { get; set; }
    public string? Comment { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Created;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? UpdatedAt { get; set; } = null;
    public DateTimeOffset? DeletedAt { get; set; } = null!;
}
