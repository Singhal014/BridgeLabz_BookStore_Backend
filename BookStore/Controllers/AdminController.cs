using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserBL _userBL;
        public AdminController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register-admin")]
        public IActionResult RegisterAdmin([FromBody] RegisterModel model)
        {
            var user = _userBL.RegisterAdmin(model); 
            if (user == null)
            {
                return BadRequest(new { message = "Registration failed!" });
            }
            return Ok(new { message = "Admin registered successfully!", user });
        }
    }
}
