using ModelLayer.Models;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        Task<UserModel> RegisterUserAsync(RegisterModel model);
        Task<UserModel> RegisterAdminAsync(RegisterModel model);
        Task<UserModel> LoginAsync(LoginModel model);
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordModel model);
    }
}