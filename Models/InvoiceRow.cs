 namespace ASP_NET_Final_Proj.Models;

public class InvoiceRow
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; } // Foreign key
    public Invoice Invoice { get; set; } // Navigation prop
    public string Service {  get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal Sum { get; set; }
}
