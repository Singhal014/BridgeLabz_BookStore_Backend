using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserBL(IUserRL userRL, IConfiguration configuration, IEmailService emailService)
        {
            _userRL = userRL;
            _configuration = configuration;
            _emailService = emailService;
        }

        public UserEntity Register(RegisterModel model)
        {
            var userEntity = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password
            };
            return _userRL.Register(userEntity);
        }

        public UserEntity Login(LoginModel model)
        {
            return _userRL.Login(model.Email, model.Password);
        }

        public string ForgotPassword(string email)
        {
            var user = _userRL.GetUserByEmail(email);
            if (user == null) throw new InvalidOperationException("User not found.");

            string token = GenerateJwtToken(user.Email);
            string emailBody = $"Your password reset token: {token}";


            _emailService.SendEmail(email, "Password Reset", emailBody);
            return token;
        }

        public bool ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters");

            var email = ExtractEmail(token);
            if (string.IsNullOrEmpty(email))
                throw new InvalidOperationException("Invalid token.");

            var user = _userRL.GetUserByEmail(email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            _userRL.UpdatePassword(user, newPassword);
            return true;
        }

        private string GenerateJwtToken(string email)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email)
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
                signingCredentials: new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
  


        private string ExtractEmail(string token)
        {
            try
            {
                var claims = DecodeJwtToken(token);
                if (claims.ContainsKey(ClaimTypes.Email))
                {
                    return claims[ClaimTypes.Email]; 
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private Dictionary<string, string> DecodeJwtToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
