using RepoLayer.Entity;
using RepoLayer.Interfaces;
using BusinessLayer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class AddressBL : IAddressBL
    {
        private readonly IAddressRL _addressRL;
        private readonly ILogger<AddressBL> _logger;

        public AddressBL(IAddressRL addressRL, ILogger<AddressBL> logger)
        {
            _addressRL = addressRL;
            _logger = logger;
        }

        public async Task<AddressEntity> AddAddressAsync(AddressEntity address)
        {
            try
            {
                _logger.LogInformation("Adding a new address for userId: {UserId}", address.UserId);
                return await _addressRL.AddAddressAsync(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding address for userId: {UserId}", address.UserId);
                throw;
            }
        }

        public async Task<IEnumerable<AddressEntity>> GetAllAddressesAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching all addresses for userId: {UserId}", userId);
                return await _addressRL.GetAllAddressesAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all addresses for userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<AddressEntity> GetAddressAsync(int userId, int addressId)
        {
            try
            {
                _logger.LogInformation("Fetching address {AddressId} for userId: {UserId}", addressId, userId);
                return await _addressRL.GetAddressAsync(userId, addressId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching address {AddressId} for userId: {UserId}", addressId, userId);
                throw;
            }
        }

        public async Task<AddressEntity> UpdateAddressAsync(int userId, int addressId, AddressEntity updatedAddress)
        {
            try
            {
                _logger.LogInformation("Updating address {AddressId} for userId: {UserId}", addressId, userId);
                return await _addressRL.UpdateAddressAsync(userId, addressId, updatedAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating address {AddressId} for userId: {UserId}", addressId, userId);
                throw;
            }
        }

        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            try
            {
                _logger.LogInformation("Deleting address {AddressId} for userId: {UserId}", addressId, userId);
                return await _addressRL.DeleteAddressAsync(userId, addressId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting address {AddressId} for userId: {UserId}", addressId, userId);
                throw;
            }
        }

        public async Task<AddressEntity> LatestAddressAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching the latest address for userId: {UserId}", userId);
                var userAddresses = await _addressRL.GetAllAddressesAsync(userId);

                if (!userAddresses.Any())
                {
                    _logger.LogWarning("No addresses found for userId: {UserId}", userId);
                    return null;
                }

                var latestAddress = userAddresses.OrderByDescending(a => a.Id).FirstOrDefault();
                _logger.LogInformation("Latest address {AddressId} retrieved for userId: {UserId}", latestAddress?.Id, userId);
                return latestAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving latest address for userId: {UserId}", userId);
                throw;
            }
        }
    }
}
