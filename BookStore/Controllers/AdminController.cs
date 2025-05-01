using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using System;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserBL userBL, ILogger<AdminController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new Admin in the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            try
            {
                _logger.LogInformation("Admin registration request received for email: {Email}", model.Email);

                var user = await _userBL.RegisterAdminAsync(model);

                if (user == null)
                {
                    _logger.LogWarning("Admin registration failed for email: {Email}", model.Email);
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Registration failed!",
                        Data = null
                    });
                }

                _logger.LogInformation("Admin registered successfully with email: {Email}", user.Email);
                return Ok(new ResponseModel<UserModel>
                {
                    Success = true,
                    Message = "Admin registered successfully!",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during admin registration for email: {Email}", model.Email);
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Registration failed due to an internal error.",
                    Data = null
                });
            }
        }
    }
}