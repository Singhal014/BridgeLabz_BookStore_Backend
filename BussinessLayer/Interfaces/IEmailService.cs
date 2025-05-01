using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}