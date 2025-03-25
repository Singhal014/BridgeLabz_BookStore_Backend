using RepoLayer.Entity;

namespace RepoLayer.Interfaces
{
    public interface IUserRL
    {
        UserEntity Register(UserEntity user);
        UserEntity Login(string email, string password);
        UserEntity GetUserByEmail(string email);
        void UpdatePassword(UserEntity user, string newPassword);
    }
}
