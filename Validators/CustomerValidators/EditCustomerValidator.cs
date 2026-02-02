using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using FluentValidation;

namespace ASP_NET_Final_Proj.Validators.CustomerValidators;

public class EditCustomerValidator: AbstractValidator<EditCustomerDto>
{
    public EditCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required")
            .MinimumLength(3).WithMessage("Customer name must be at least 3 characters long");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Wrong email address syntax");
        RuleFor(x => x.PhoneNumber)
            .MinimumLength(10).WithMessage("Phone number must be at least 10 characters long");
    }
}
