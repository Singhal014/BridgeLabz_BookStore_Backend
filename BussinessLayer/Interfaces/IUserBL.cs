using ModelLayer.Models;
using RepoLayer.Entity;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        UserEntity Register(RegisterModel model);
        UserEntity Login(LoginModel model);
        string ForgotPassword(string email);
        bool ResetPassword(string token, string newPassword);
    }
}