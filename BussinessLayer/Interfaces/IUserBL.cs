using ModelLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        UserModel RegisterUser(RegisterModel model);  
        UserModel RegisterAdmin(RegisterModel model); 
        UserModel Login(LoginModel model);
        string ForgotPassword(string email);
        bool ResetPassword(ResetPasswordModel model);
    }
}
