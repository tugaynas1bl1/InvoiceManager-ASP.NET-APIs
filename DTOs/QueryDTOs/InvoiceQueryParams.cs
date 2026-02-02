namespace ASP_NET_Final_Proj.DTOs.QueryDTOs;

public class InvoiceQueryParams
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Sort { get; set; }
    public string? SortDirection { get; set; } = "desc";
    public string? Status { get; set; }
    public string? Search { get; set; }
    public Guid? CustomerId { get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 10;
        if (Size > 100) Size = 100;
        if (string.IsNullOrEmpty(SortDirection)) SortDirection = "asc";
        SortDirection = SortDirection.ToLower();
        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "asc";
    }
}
