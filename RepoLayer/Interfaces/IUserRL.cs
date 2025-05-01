using RepoLayer.Entity;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IUserRL
    {
        Task<UserEntity> RegisterAsync(UserEntity user);
        Task<UserEntity> GetUserByEmailAsync(string email);
        Task UpdatePasswordAsync(UserEntity user);
        Task<UserEntity> GetUserByIdAsync(int userId);
    }
}