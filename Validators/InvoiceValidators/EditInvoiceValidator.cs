using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.Models;
using FluentValidation;

namespace ASP_NET_Final_Proj.Validators.InvoiceValidators;

public class EditInvoiceValidator : AbstractValidator<EditInvoiceDto>
{
    public EditInvoiceValidator()
    {
        RuleFor(x => x.Comment)
           .MinimumLength(10).WithMessage("Invoice comment must be at least 3 characters long");
        RuleFor(x => x.Status)
            .Must(p => new[] { InvoiceStatus.Created, InvoiceStatus.Sent, InvoiceStatus.Paid, InvoiceStatus.Cancelled, InvoiceStatus.Received, InvoiceStatus.Rejected }.Contains(p))
            .WithMessage("Invoice Status must be one of: 0(Created), 1(Sent), 2(Received), 3(Paid), 4(Cancelled), 5(Rejected)");
    }
}
