using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class AddressRL : IAddressRL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddressRL> _logger;

        public AddressRL(ApplicationDbContext context, ILogger<AddressRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AddressEntity> AddAddressAsync(AddressEntity address)
        {
            try
            {
                _logger.LogInformation("Adding new address for user {UserId}", address.UserId);
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Address added successfully with ID {AddressId}", address.Id);
                return address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding address for user {UserId}", address.UserId);
                throw;
            }
        }

        public async Task<IEnumerable<AddressEntity>> GetAllAddressesAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching all addresses for user {UserId}", userId);
                return await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving addresses for user {UserId}", userId);
                throw;
            }
        }

        public async Task<AddressEntity> GetAddressAsync(int userId, int addressId)
        {
            try
            {
                _logger.LogInformation("Fetching address {AddressId} for user {UserId}", addressId, userId);
                return await _context.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving address {AddressId} for user {UserId}", addressId, userId);
                throw;
            }
        }

        public async Task<AddressEntity> UpdateAddressAsync(int userId, int addressId, AddressEntity updatedAddress)
        {
            try
            {
                _logger.LogInformation("Updating address {AddressId} for user {UserId}", addressId, userId);
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);

                if (address != null)
                {
                    address.FirstName = updatedAddress.FirstName;
                    address.LastName = updatedAddress.LastName;
                    address.PhoneNo = updatedAddress.PhoneNo;
                    address.PinCode = updatedAddress.PinCode;
                    address.City = updatedAddress.City;
                    address.State = updatedAddress.State;
                    address.Address = updatedAddress.Address;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Address {AddressId} updated successfully", addressId);
                    return address;
                }

                _logger.LogWarning("Address {AddressId} not found for user {UserId}", addressId, userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating address {AddressId} for user {UserId}", addressId, userId);
                throw;
            }
        }

        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            try
            {
                _logger.LogInformation("Deleting address {AddressId} for user {UserId}", addressId, userId);
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);

                if (address != null)
                {
                    _context.Addresses.Remove(address);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Address {AddressId} deleted successfully", addressId);
                    return true;
                }

                _logger.LogWarning("Address {AddressId} not found for user {UserId}", addressId, userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting address {AddressId} for user {UserId}", addressId, userId);
                throw;
            }
        }
    }
}
