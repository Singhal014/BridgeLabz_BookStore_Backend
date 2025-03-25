using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepoLayer.Entity;

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
            var user = _userBL.Register(model);
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
                var token = _userBL.ForgotPassword(model.Email);
                return Ok(new
                {
                    Success = true,
                    Message = "Password reset link sent to your email.",
                    Token = token 
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
        public IActionResult ResetPassword([FromQuery] string token, [FromBody] NewPasswordModel model)
        {
            try
            {
                bool isReset = _userBL.ResetPassword(token, model.NewPassword);
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
