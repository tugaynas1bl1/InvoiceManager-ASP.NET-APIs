using ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.Models;
using FluentValidation;

namespace ASP_NET_Final_Proj.Validators.InvoiceValidators;

public class CreateInvoiceRowValidator : AbstractValidator<CreateInvoiceRowDto>
{
    public CreateInvoiceRowValidator()
    {
        RuleFor(x => x.Service)
            .NotEmpty()
            .MinimumLength(10)
            .WithMessage("Service must be at least 3 characters long");
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("Invoice id should be written");
    }
}
