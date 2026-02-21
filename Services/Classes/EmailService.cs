using ASP_NET_Final_Proj.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace ASP_NET_Final_Proj.Services.Classes;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, int verificationCode)
    {
            var settings = _configuration.GetSection("EmailSettings");
            var issuerEmail = settings["Email"];
            var appPasword = settings["AppPassword"];
            var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587);
            client.Authenticate(issuerEmail, appPasword);

            var message = new MimeKit.MimeMessage();

            message.From.Add(new MailboxAddress("Verification", issuerEmail));

            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Verification Code for Password Change";

            message.Body = new TextPart("html")
            {
                Text = $"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <title>Password Verification</title>
    </head>
    <body style="font-family: Arial, sans-serif; background-color: #f7f7f7; margin:0; padding:0;">
        <table align="center" width="100%" cellpadding="0" cellspacing="0" style="max-width:600px; margin-top:40px; background-color:#ffffff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.1);">
            <tr>
                <td style="padding:20px; text-align:center; background-color:#0d6efd; color:#ffffff; border-top-left-radius:8px; border-top-right-radius:8px;">
                    <h2>Password Verification Code</h2>
                </td>
            </tr>
            <tr>
                <td style="padding:30px; text-align:center; color:#333333;">
                    <p style="font-size:16px;">Hello,</p>
                    <p style="font-size:16px;">Use the following verification code to complete your password change:</p>
                
                    <div style="margin:20px auto; display:inline-block; padding:15px 25px; background-color:#f1f1f1; border-radius:6px; font-size:24px; font-weight:bold; letter-spacing:2px; color:#0d6efd;">
                        {verificationCode}
                    </div>

                    <p style="font-size:14px; color:#666666; margin-top:20px;">
                        This code will expire in 10 minutes. <br/>
                        If you did not request a password change, please ignore this email.
                    </p>
                </td>
            </tr>
            <tr>
                <td style="padding:20px; text-align:center; font-size:12px; color:#999999;">
                    &copy; 2026 YourAppName. All rights reserved.
                </td>
            </tr>
        </table>
    </body>
    </html>
    """
            };


            client.Send(message);
            client.Disconnect(true);
    }

}
