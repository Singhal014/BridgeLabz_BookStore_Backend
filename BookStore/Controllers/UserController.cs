using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var user = _userBL.RegisterUser(model);
            if (user == null)
            {
                return BadRequest(new { message = "Registration failed!" });
            }
            return Ok(new { message = "User registered successfully!", user });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _userBL.Login(model);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            return Ok(new { message = "Login successful", user });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            try
            {
                var result = _userBL.ForgotPassword(model.Email);
                return Ok(new
                {
                    Success = true,
                    Message = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                bool isReset = _userBL.ResetPassword(model);

                if (isReset)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Password reset successfully."
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = "Password reset failed."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
