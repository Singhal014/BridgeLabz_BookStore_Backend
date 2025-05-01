using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IPasswordHasherBL
    {
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyPasswordAsync(string inputPassword, string storedHash);
    }
}