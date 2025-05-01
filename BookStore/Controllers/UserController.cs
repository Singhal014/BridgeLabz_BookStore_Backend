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
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            _logger.LogInformation("Registering new user: {Email}", model.Email);

            try
            {
                var user = await _userBL.RegisterUserAsync(model);
                if (user == null)
                {
                    _logger.LogWarning("Registration failed for user: {Email}", model.Email);
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Registration failed!",
                        Data = null
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", user.Email);
                return Ok(new ResponseModel<UserModel>
                {
                    Success = true,
                    Message = "User registered successfully!",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", model.Email);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Logs in an existing user and admin.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("User attempting login: {Email}", model.Email);

            try
            {
                var user = await _userBL.LoginAsync(model);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: Invalid email or password for {Email}", model.Email);
                    return Unauthorized(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Data = null
                    });
                }

                _logger.LogInformation("User logged in successfully: {Email}", user.Email);
                return Ok(new ResponseModel<UserModel>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Email}", model.Email);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Sends an OTP to the user's email for password reset.
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            _logger.LogInformation("Processing forgot password request for: {Email}", model.Email);

            try
            {
                var result = await _userBL.ForgotPasswordAsync(model.Email);
                _logger.LogInformation("OTP sent successfully to: {Email}", model.Email);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = result,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password request failed for: {Email}", model.Email);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Resets the user's password using an OTP, email, and new password.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            _logger.LogInformation("Processing password reset for: {Email}", model.Email);

            try
            {
                bool isReset = await _userBL.ResetPasswordAsync(model);

                if (isReset)
                {
                    _logger.LogInformation("Password reset successfully for: {Email}", model.Email);
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Password reset successfully.",
                        Data = null
                    });
                }

                _logger.LogWarning("Password reset failed for: {Email}", model.Email);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Password reset failed.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset error for: {Email}", model.Email);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}