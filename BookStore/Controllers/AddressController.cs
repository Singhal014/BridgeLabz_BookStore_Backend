using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using RepoLayer.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressBL _addressBL;
        private readonly ILogger<AddressController> _logger;

        public AddressController(IAddressBL addressBL, ILogger<AddressController> logger)
        {
            _addressBL = addressBL;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new address for the authenticated user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddAddressAsync([FromBody] AddressModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Attempted to add a null address.");
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address data is null.",
                        Data = null
                    });
                }

                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Adding address for userId: {UserId}", userId);

                var addressEntity = new AddressEntity
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNo = model.PhoneNo,
                    PinCode = model.PinCode,
                    City = model.City,
                    State = model.State,
                    Address = model.Address
                };

                var address = await _addressBL.AddAddressAsync(addressEntity);

                if (address == null)
                {
                    _logger.LogWarning("Failed to add address for userId: {UserId}", userId);
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address could not be added.",
                        Data = null
                    });
                }

                _logger.LogInformation("Address added successfully for userId: {UserId}", userId);
                return Ok(new ResponseModel<AddressEntity>
                {
                    Success = true,
                    Message = "Address added successfully!",
                    Data = address
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding address.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves all addresses for the authenticated user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAddressesAsync()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Retrieving all addresses for userId: {UserId}", userId);

                var addresses = await _addressBL.GetAllAddressesAsync(userId);

                if (addresses == null || !addresses.Any())
                {
                    _logger.LogWarning("No addresses found for userId: {UserId}", userId);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "No addresses found for this user.",
                        Data = null
                    });
                }

                _logger.LogInformation("Addresses retrieved successfully for userId: {UserId}", userId);
                return Ok(new ResponseModel<IEnumerable<AddressEntity>>
                {
                    Success = true,
                    Message = "Addresses retrieved successfully.",
                    Data = addresses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving addresses.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Updates an existing address for the authenticated user
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateAddressAsync(int addressId, [FromBody] AddressModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Attempted to update a null address.");
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address data is null.",
                        Data = null
                    });
                }

                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Updating address {AddressId} for userId: {UserId}", addressId, userId);

                var updatedAddressEntity = new AddressEntity
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNo = model.PhoneNo,
                    PinCode = model.PinCode,
                    City = model.City,
                    State = model.State,
                    Address = model.Address
                };

                var updatedAddress = await _addressBL.UpdateAddressAsync(userId, addressId, updatedAddressEntity);

                if (updatedAddress == null)
                {
                    _logger.LogWarning("Address {AddressId} not found or update failed for userId: {UserId}", addressId, userId);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address not found or update failed.",
                        Data = null
                    });
                }

                _logger.LogInformation("Address {AddressId} updated successfully for userId: {UserId}", addressId, userId);
                return Ok(new ResponseModel<AddressEntity>
                {
                    Success = true,
                    Message = "Address updated successfully!",
                    Data = updatedAddress
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating address {AddressId}", addressId);
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Deletes a specific address for the authenticated user
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAddressAsync(int addressId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Deleting address {AddressId} for userId: {UserId}", addressId, userId);

                bool isDeleted = await _addressBL.DeleteAddressAsync(userId, addressId);

                if (!isDeleted)
                {
                    _logger.LogWarning("Address {AddressId} not found or delete failed for userId: {UserId}", addressId, userId);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address not found or delete failed.",
                        Data = null
                    });
                }

                _logger.LogInformation("Address {AddressId} deleted successfully for userId: {UserId}", addressId, userId);
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Address deleted successfully!",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting address {AddressId}", addressId);
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }
    }
}