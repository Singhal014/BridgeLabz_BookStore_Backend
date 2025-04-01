using RepoLayer.Entity;

namespace RepoLayer.Interfaces
{
    public interface IUserRL
    {
        UserEntity Register(UserEntity user);
        UserEntity GetUserByEmail(string email);
        void UpdatePassword(UserEntity user);
        UserEntity GetUserById(int userId);
    }
}
