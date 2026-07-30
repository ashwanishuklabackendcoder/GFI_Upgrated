using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Helpers;

public static class EmailSender
{
    public static async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var fromAddress = new MailAddress("info@creativ-eras.com", "GFI Portal");
            var toAddress = new MailAddress(toEmail);

            using var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("info@creativ-eras.com", "lofy iwgy qoys dhoo")
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Log the error locally to stdout/diagnostics
            Console.WriteLine($"[EmailSender Error]: {ex.Message}");
        }
    }
}
