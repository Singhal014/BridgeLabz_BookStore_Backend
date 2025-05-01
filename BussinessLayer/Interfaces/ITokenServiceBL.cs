using RepoLayer.Entity;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ITokenServiceBL
    {
        Task<string> GenerateJwtTokenAsync(UserEntity user);
    }
}