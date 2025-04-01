using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private static Dictionary<string, (string otp, DateTime expiry)> _otpStorage = new Dictionary<string, (string, DateTime)>();

        public UserBL(IUserRL userRL, IConfiguration configuration, IEmailService emailService)
        {
            _userRL = userRL;
            _configuration = configuration;
            _emailService = emailService;
        }

        public UserModel RegisterUser(RegisterModel model)
        {
            return Register(model, "User");
        }

        public UserModel RegisterAdmin(RegisterModel model)
        {
            return Register(model, "Admin");
        }

        private UserModel Register(RegisterModel model, string role)
        {
            var hashedPassword = HashPassword(model.Password);

            var userEntity = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = hashedPassword,
                Role = role
            };

            var createdUser = _userRL.Register(userEntity);
            return MapUserEntityToModel(createdUser);
        }

        public UserModel Login(LoginModel model)
        {
            var user = _userRL.GetUserByEmail(model.Email);
            if (user == null || !VerifyPassword(model.Password, user.Password))
                return null;

            user.Token = GenerateJwtToken(user);

            return new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = user.Token
            };
        }

        public string ForgotPassword(string email)
        {
            var user = _userRL.GetUserByEmail(email);
            if (user == null) throw new InvalidOperationException("User not found.");

            string otp = GenerateOtp();
            _otpStorage[email] = (otp, DateTime.Now.AddMinutes(10));

            string emailBody = $"Your password reset OTP: {otp}. This OTP is valid for 10 minutes.";
            _emailService.SendEmail(email, "Password Reset OTP", emailBody);

            return "OTP sent to your email.";
        }

        public bool ResetPassword(ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters");

            if (!_otpStorage.TryGetValue(model.Email, out var otpData))
                throw new InvalidOperationException("OTP not found or expired.");

            if (otpData.otp != model.Otp)
                throw new InvalidOperationException("Invalid OTP.");

            if (DateTime.Now > otpData.expiry)
            {
                _otpStorage.Remove(model.Email);
                throw new InvalidOperationException("OTP has expired.");
            }

            var hashedPassword = HashPassword(model.NewPassword);

            var user = _userRL.GetUserByEmail(model.Email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            user.Password = hashedPassword;
            _userRL.UpdatePassword(user);
            _otpStorage.Remove(model.Email);

            return true;
        }

        private UserModel MapUserEntityToModel(UserEntity user)
        {
            return new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = user.Token
            };
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return string.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
