using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IOtpServiceBL
    {
        Task<string> GenerateOtpAsync();
        Task StoreOtpAsync(string email, string otp);
        Task<bool> ValidateOtpAsync(string email, string otp);
    }
}