using FluentValidation;
using System.Text.RegularExpressions;

namespace ASP_NET_Final_Proj.Validators;

public static class ValidationRulesExtension
{
    public static IRuleBuilder<T, string> Password<T>(
        this IRuleBuilder<T, 
            string> ruleBuilder,
        bool mustContainLowerCase = true,
        bool mustContainUpperCase = true,
        bool mustContainDigit = true)
    {
        return ruleBuilder
            .Must(password =>
            {
                if (mustContainLowerCase && !Regex.IsMatch(password, @"[a-z]"))
                    return false;
                if (mustContainUpperCase && !Regex.IsMatch(password, @"[A-Z]"))
                    return false;
                if (mustContainDigit && !Regex.IsMatch(password, @"\d"))
                    return false;
                return true;
            }).WithMessage("Password must have at least one decimal, one lowercase('a'-'z'), and also one uppercase ('A'-'Z')");
    }
}
