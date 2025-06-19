using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace BookShoppingCartMVC.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This is a placeholder method.
            // In production, you can integrate SMTP, SendGrid, etc.

            // For now, simulate email sending.
            System.Diagnostics.Debug.WriteLine("Sending Email...");
            System.Diagnostics.Debug.WriteLine($"To: {email}");
            System.Diagnostics.Debug.WriteLine($"Subject: {subject}");
            System.Diagnostics.Debug.WriteLine($"Message: {htmlMessage}");

            return Task.CompletedTask;
        }
    }
}
