namespace ASP_NET_Final_Proj.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, int verificationCode);
}
