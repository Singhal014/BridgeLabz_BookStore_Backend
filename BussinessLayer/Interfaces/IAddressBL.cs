using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAddressBL
    {
        Task<AddressEntity> AddAddressAsync(AddressEntity address);
        Task<IEnumerable<AddressEntity>> GetAllAddressesAsync(int userId);
        Task<AddressEntity> GetAddressAsync(int userId, int addressId);
        Task<AddressEntity> UpdateAddressAsync(int userId, int addressId, AddressEntity updatedAddress);
        Task<bool> DeleteAddressAsync(int userId, int addressId);
        Task<AddressEntity> LatestAddressAsync(int userId);
    }
}